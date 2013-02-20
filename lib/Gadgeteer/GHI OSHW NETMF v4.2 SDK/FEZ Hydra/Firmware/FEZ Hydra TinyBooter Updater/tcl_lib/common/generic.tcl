#  ----------------------------------------------------------------------------
#          ATMEL Microcontroller Software Support
#  ----------------------------------------------------------------------------
#  Copyright (c) 2008, Atmel Corporation
#
#  All rights reserved.
#
#  Redistribution and use in source and binary forms, with or without
#  modification, are permitted provided that the following conditions are met:
#
#  - Redistributions of source code must retain the above copyright notice,
#  this list of conditions and the disclaimer below.
#
#  Atmel's name may not be used to endorse or promote products derived from
#  this software without specific prior written permission. 
#
#  DISCLAIMER: THIS SOFTWARE IS PROVIDED BY ATMEL "AS IS" AND ANY EXPRESS OR
#  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
#  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT ARE
#  DISCLAIMED. IN NO EVENT SHALL ATMEL BE LIABLE FOR ANY DIRECT, INDIRECT,
#  INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
#  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
#  OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
#  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
#  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
#  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#  ----------------------------------------------------------------------------

################################################################################
################################################################################
## NAMESPACE GENERIC
################################################################################
################################################################################
namespace eval GENERIC {

    # Following variables are updated by the Init function
    variable appletAddr
    variable appletMailboxAddr
    
    # Following variables are set by the Init function
    variable appletBufferAddress
    variable appletBufferSize
    variable memorySize   0
    variable startAddress 0

    # Standard applet commands
    array set appletCmd {
        init            0x00
        erase           0x01
        write           0x02
        read            0x03
        lock            0x04
        unlock          0x05
        gpnvm           0x06
        security        0x07
        erasebuffer     0x08
        binarypage      0x09
        listbadblocks   0x10
        tagBlock        0x11
        readUniqueID    0x12
        eraseBlocks     0x13
        batchErase      0x14
    }

    #===============================================================================
    # Set trace level for applets
    # 5 : trace_NONE    - No trace will be printed
    # 4 : trace_FATAL   - Indicates a major error which prevents the program from
    #                     going any further.
    # 3 : trace_ERROR   - Indicates an error which may not stop the program
    #                     execution, but which indicates there is a problem with
    #                     the code.
    # 2 : trace_WARNING - Indicates that a minor error has happened. In most case
    #                     it can be discarded safely; it may even be expected.
    # 1 : trace_INFO    - Informational trace about the program execution. Should
    #                     enable the user to see the execution flow.
    # 0 : trace_DEBUG   - Traces whose only purpose is for debugging the program,
    #                     and which do not produce meaningful information otherwise.
    
}



#===============================================================================
#  proc GENERIC::LoadApplet
#-------------------------------------------------------------------------------
proc GENERIC::LoadApplet {appletAddr appletFileName} {
    global target
    global libPath

    puts "-I- Loading applet [file tail $appletFileName] at address [format "0x%X" $appletAddr]"
    
    # Open Data Flash Write file
    if { [catch {set f [open $appletFileName r]}] } {
        error "Can't open file $appletFileName"
    }

    # Copy applet into Memory at the  appletAddr 
    fconfigure $f -translation binary
    set size [file size $appletFileName]
    set appletBinary [read $f $size]
    if {[catch {TCL_Write_Data $target(handle) $appletAddr appletBinary $size dummy_err} dummy_err]} {
        error "Can't write applet $appletFileName"
    }
    close $f    
}

#===============================================================================
#  proc GENERIC::Run
#
#  Launch the applet, wait for the end of execution and return the result
#-------------------------------------------------------------------------------
proc GENERIC::Run { cmd } {
    global target
    variable appletAddr
    variable appletMailboxAddr
    
    set appletCmdAddr    [expr $appletMailboxAddr]
    set appletStatusAddr [expr $appletMailboxAddr + 4]
    
#    puts "-I- Running applet command $cmd at address [format "0x%X" $appletAddr]"

    # Launch the applet Jumping to the appletAddr
    if {[catch {TCL_Go $target(handle) $appletAddr} dummy_err] } {
        error "Error Running the applet"
    }
    
    # Wait for the end of execution
    # TO DO: Handle timeout error
    set result $cmd
    while {$result != [expr ~($cmd)]} {
        if {[catch {set result [TCL_Read_Int $target(handle) $appletCmdAddr]} dummy_err] } {
            error "Error polling the end of applet execution"
        }
    }
        
    # Return the error code returned by the applet
    if {[catch {set result [TCL_Read_Int $target(handle) $appletStatusAddr]} dummy_err] } {
        error "Error reading the applet result"
    }
    
    return $result
}

#===============================================================================
#  proc GENERIC::Write
#-------------------------------------------------------------------------------
proc GENERIC::Write {offset sizeToWrite File isBootFile} {
    global target
    variable appletArg
    variable appletCmd
    variable appletAddr
    variable appletMailboxAddr
    variable appletBufferAddress

    global target
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    set maxBufferSize [expr $GENERIC::appletBufferSize]
        
    # Sanity check: buffer to write should not exceed the whole memory size    
    if { [expr $offset + $sizeToWrite] > $GENERIC::memorySize } {
        error "File size exceed memory capacity ([format "0x%X" $GENERIC::memorySize] bytes)"
    }
    
    # Init the ping pong algorithm: the buffer is active
    set bufferAddress $GENERIC::appletBufferAddress
    
    # Write data page after page...
    while {$sizeToWrite > 0} {
        # Adjust the packet size to be sent
        if {$sizeToWrite < $maxBufferSize} {
            set bufferSize $sizeToWrite
        } else {
            set bufferSize $maxBufferSize
        }
        
        # Read Data From Input File
        set rawData [read $File $bufferSize]
    
        puts "-I- \tWriting: [format "0x%X" $bufferSize] bytes at [format "0x%X" $offset] (buffer addr : [format "0x%X" $bufferAddress])"
    
        # Copy in RAM the content of the page to be written
        TCL_Write_Data $target(handle) $bufferAddress rawData $bufferSize dummy_err
        
        # If this is a boot file modify 6th vector with file size
        if {[expr ($isBootFile == 1) && ($offset == 0)]} {
            TCL_Write_Int $target(handle) $sizeToWrite [expr $bufferAddress + (5 * 4)] dummy_err
        }
        
        # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $appletCmd(write) $appletAddrCmd} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $bufferAddress $appletAddrArgv0} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }
        # Write the buffer size in the argument area
        if {[catch {TCL_Write_Int $target(handle) $bufferSize $appletAddrArgv1} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }
        # Write the memory offset in the argument area
        if {[catch {TCL_Write_Int $target(handle) $offset $appletAddrArgv2} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }

        # Launch the applet Jumping to the appletAddr
        if {[catch {set result [GENERIC::Run $appletCmd(write)]} dummy_err]} {
            error "[format "0x%08x" $dummy_err]"
        }
        switch $result {
            0 {
                # APPLET_SUCCESS
                # Retrieve how many bytes have been written
                set sizeWritten [TCL_Read_Int $target(handle) $appletAddrArgv0]
                
                puts "-I- \t[format "0x%X" $sizeWritten] bytes written by applet"
                
                # If all the buffer has not been written, move back to the first un-written byte offset
                if {$sizeWritten < $bufferSize} {
                    seek $File [expr $sizeWritten - $bufferSize] current
                }
                
                incr sizeToWrite [expr -$sizeWritten]
                incr offset $sizeWritten
            }
            
            1 {
                # APPLET_DEV_UNKNOWN
                error "[format "0x%08x" $result]"
            }
            
            9 {
                # APPLET_BAD_BLOCK
                # BAD block strategy : block is skipped and we write the next block with same data

                # Retrieve how many bytes have been written (must be 0)
                set sizeWritten  [TCL_Read_Int $target(handle) $appletAddrArgv0]
                
                puts "-I- \t[format "0x%X" $sizeWritten] bytes written by applet"
                
                # If all the buffer has not been written, move back to the first un-written byte offset
                seek $File [expr -$bufferSize] current
                
                incr offset $maxBufferSize
            }

            default {
                # All other errors
                error "[format "0x%08x" $result]"
            }
        }
    }

    return 1
}

#===============================================================================
#  proc GENERIC::Read
#-------------------------------------------------------------------------------
proc GENERIC::Read { offset sizeToRead File } {
    global target
    variable appletArg
    variable appletCmd
    variable appletAddr
    variable appletMailboxAddr
    variable appletBufferAddress
    variable appletBufferSize

    global target
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    set maxBufferSize [expr $GENERIC::appletBufferSize]
        
    # Sanity check: buffer to read should not exceed the whole memory size    
    if { [expr $offset + $sizeToRead] > $GENERIC::memorySize } {
        error "File size exceed memory capacity ([format "0x%X" $GENERIC::memorySize] bytes)"
    }
    
    # Init the ping pong algorithm: the buffer is active
    set bufferAddress $GENERIC::appletBufferAddress
    
    # Write data page after page...
    while {$sizeToRead > 0} {
        # Adjust the packet size to be sent
        if {$sizeToRead < $maxBufferSize} {
            set bufferSize $sizeToRead
        } else {
            set bufferSize $maxBufferSize
        }
        
        puts "-I- \tReading: [format "0x%X" $bufferSize] bytes at [format "0x%X" $offset] (buffer addr : [format "0x%X" $bufferAddress])"
    
         # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $appletCmd(read) $appletAddrCmd} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $bufferAddress $appletAddrArgv0} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }
        # Write the buffer size in the argument area
        if {[catch {TCL_Write_Int $target(handle) $bufferSize $appletAddrArgv1} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }
        # Write the memory offset in the argument area
        if {[catch {TCL_Write_Int $target(handle) $offset $appletAddrArgv2} dummy_err] } {
            error "[format "0x%08x" $dummy_err]"
        }

        # Launch the applet Jumping to the appletAddr
        if {[catch {set result [GENERIC::Run $appletCmd(read)]} dummy_err]} {
            error "[format "0x%08x" $dummy_err]"
        }
        switch $result {
            0 {
                # APPLET_SUCCESS
                # Retrieve how many bytes have been read
                set sizeRead [TCL_Read_Int $target(handle) $appletAddrArgv0]
                
                # Write Data to Output File
                set rawData [TCL_Read_Data $target(handle) $bufferAddress $sizeRead dummy_err]
                puts -nonewline $File $rawData    

                incr sizeToRead [expr -$sizeRead]
                incr offset $sizeRead
            }
            
            1 {
                # APPLET_DEV_UNKNOWN
                error "[format "0x%08x" $result]"
            }
            
            9 {
                # APPLET_BAD_BLOCK
                # BAD block strategy : block is skipped and we read the next block
                # Retrieve how many bytes have been read (must be 0)
                set sizeRead [TCL_Read_Int $target(handle) $appletAddrArgv0]
                
                # No Data to write in output file
                incr offset $maxBufferSize
            }

            default {
                # All other errors
                error "[format "0x%08x" $result]"
            }
        }
    }

    return 1
}

#===============================================================================
#  proc GENERIC::Init
#-------------------------------------------------------------------------------
proc GENERIC::Init {memAppletAddr memAppletMbxAddr appletFileName {appletArgList 0}} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletCmd

    # Update the current applet addresses
    set appletAddr          $memAppletAddr
    set appletMailboxAddr   $memAppletMbxAddr


    # Load the applet to the target
    if {[catch {GENERIC::LoadApplet $appletAddr $appletFileName} dummy_err]} {
        error "Applet $appletFileName can not be loaded"
    }

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    
    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $appletCmd(init) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command\n$dummy_err"
    }

    set argIdx 0
    foreach arg $appletArgList {
        # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $arg [expr $appletAddrArgv0 + $argIdx]} dummy_err] } {
            error "Error Writing Applet argument $arg ($dummy_err)"
        }
        incr argIdx 4
    }
    
    # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $appletCmd(init)]} dummy_err]} {
        error "Applet Init command has not been launched ($dummy_err)"
    }
    if {$result == 1} {
        error "Can't detect known device"
    } elseif {$result != 0} {
        error "Applet Init command returns error: [format "0x%08x" $result]"
    }
        
    # Retrieve values
    variable memorySize
    variable appletBufferAddress
    variable appletBufferSize
    set GENERIC::memorySize          [TCL_Read_Int $target(handle) $appletAddrArgv0]
    set GENERIC::appletBufferAddress [TCL_Read_Int $target(handle) $appletAddrArgv1]
    set GENERIC::appletBufferSize    [TCL_Read_Int $target(handle) $appletAddrArgv2]

    
    set FLASH::flashLockRegionSize [expr [TCL_Read_Int $target(handle) $appletAddrArgv3] & 0xFFFF]
    set FLASH::flashNumbersLockBits  [expr [TCL_Read_Int $target(handle) $appletAddrArgv3] >> 16]
    set FLASH::flashSize $GENERIC::memorySize

    puts "-I- Memory Size : [format "0x%X" $GENERIC::memorySize] bytes"
    puts "-I- Buffer address : [format "0x%X" $GENERIC::appletBufferAddress]"
    puts "-I- Buffer size: [format "0x%X" $GENERIC::appletBufferSize] bytes"
    
    puts "-I- Applet initialization done"
}

#===============================================================================
#  proc GENERIC::SendFile
#-------------------------------------------------------------------------------
proc GENERIC::SendFile {name addr {isBootFile 0}} {

    puts "GENERIC::SendFile $name at address [format "0x%X" $addr]"

    if { [catch {set f [open $name r]}] } {
        puts "-E- Can't open file $name"
        return
    }

    fconfigure $f -translation binary

    set size [file size $name]
    puts "-I- File size : [format "0x%X" $size] byte(s)"

    if {[catch {GENERIC::Write $addr $size $f $isBootFile} dummy_err]} {
        puts "-E- Can't send data ($dummy_err)"
    }
    close $f
}

#===============================================================================
#  proc GENERIC::ReceiveFile
#-------------------------------------------------------------------------------
proc GENERIC::ReceiveFile {name addr size} {

    puts "GENERIC::ReceiveFile $name : [format "0x%X" $size] bytes from address [format "0x%X" $addr]"

    # put data in a file
    if { [catch {set f2 [open $name w+]}] } {
        puts "-E- Can't open file $name"
        return -1
    }
    fconfigure $f2 -translation binary
    
    #read data from target
    if { [catch {GENERIC::Read $addr $size $f2} dummy_err] } {
        puts "-E- Can't receive data ($dummy_err)"
    }
    
    close $f2
}

#===============================================================================
#  proc GENERIC::EraseAll
#-------------------------------------------------------------------------------
proc GENERIC::EraseAll {{param 0}} {
    global target
    global softwareStatusLabelVariable
    variable appletMailboxAddr
    variable appletCmd

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $appletCmd(erase) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }

    # Write the parameter in the argument area
    if {[catch {TCL_Write_Int $target(handle) $param $appletAddrArgv0} dummy_err] } {
        error "[format "0x%08x" $dummy_err]"
    }

    set softwareStatusLabelVariable "Full Erase in progress ..."

    puts "-I- GENERIC::EraseAll"

    # Run the applet
    if {[catch {set result [GENERIC::Run $appletCmd(erase)]} dummy_err]} {
        set softwareStatusLabelVariable ""
        error "Applet Erase command has not been launched ($dummy_err)"
    }

    set softwareStatusLabelVariable ""

    if {$result != 0} {
         error "[format "0x%08x" $dummy_err]"
    }
}

#===============================================================================
#  proc GENERIC::SendBootFileGUI
#-------------------------------------------------------------------------------
proc GENERIC::SendBootFileGUI {} {
    global softwareStatusLabelVariable
    global backupDirPath
    global conWindows
    

    set fileName [tk_getOpenFile -parent $conWindows -initialdir  $backupDirPath -defaultextension ".bin" -filetypes {{"Bin Files" {.bin}} {"All Files" {*.*}}}]
    if {$fileName == ""} {
        puts "-E- No File Selected"
        return
    }
    
    if { [catch {set size [file size $fileName]}] } {
        puts "-E- Can't open file $fileName"
        return
    }
    
    if { $size > $BOARD::maxBootSize } {
        puts "-E- Unauthorized Boot File Size"
        return
    }
    
    set softwareStatusLabelVariable "Sending data ..."

    # Send the file to the target (modifying the 6th vector)
    GENERIC::SendFile $fileName 0x0 1

    #Close the wait window
    set softwareStatusLabelVariable ""

    # Store the current dir
    set backupDirPath [file dirname $fileName]
}

#===============================================================================
#  proc GENERIC::SendBootFile
#-------------------------------------------------------------------------------
proc GENERIC::SendBootFile {name} {
    GENERIC::SendFile $name 0x0 1
}


################################################################################
################################################################################
## NAMESPACE RAM
################################################################################
################################################################################
namespace eval RAM {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
}

#===============================================================================
#  proc RAM::sendFile
#-------------------------------------------------------------------------------
proc RAM::sendFile { name addr } {
    
    global valueOfDataForSendFile
    global target
    set dummy_err 0

    if {[catch {set f [open $name r]}]} {
        set valueOfDataForSendFile 0
        puts "-E- Can't open file $name"
        return -1
    }
    
    fconfigure $f -translation binary
    
    set size [file size $name]
    puts "-I- File size = $size byte(s)"
    set valueOfDataForSendFile [read $f $size]
    
    close $f

    if {[catch {TCL_Write_Data $target(handle) $addr valueOfDataForSendFile $size dummy_err}]} {
        puts "-E- Can't send data, error in connection"
        set valueOfDataForSendFile 0
        return
    }
    
    set valueOfDataForSendFile 0
}

#===============================================================================
#  proc RAM::receiveFile
#-------------------------------------------------------------------------------
proc RAM::receiveFile {name addr size} {

    global target
    set dummy_err 0

    #read data from target
    if {[catch {set result [TCL_Read_Data $target(handle) $addr $size dummy_err]}]} {
        puts "-E- Can't read data, error in connection"
        return -1
    }

    # put data in a file
    if {[catch {set f2 [open $name w+]}]} {
        puts "-E- Can't open file $name"
        return -1
    }

    fconfigure $f2 -translation binary
    puts -nonewline $f2 $result
    close $f2
}


################################################################################
## NAMESPACE DATAFLASH
################################################################################
namespace eval DATAFLASH {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
    variable DATAFLASH_initialized 0
}

#===============================================================================
#  proc DATAFLASH::Init
#-------------------------------------------------------------------------------
proc DATAFLASH::Init {dfId} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
    variable DATAFLASH_initialized

    set DATAFLASH_initialized 0

    puts "-I- DATAFLASH::Init $dfId (trace level : $target(traceLevel))"
    
    # Load the applet to the target
    if {[catch {GENERIC::Init $DATAFLASH::appletAddr $DATAFLASH::appletMailboxAddr $DATAFLASH::appletFileName [list $target(comType) $target(traceLevel) $dfId ]} dummy_err] } {
        error "Error Initializing DataFlash Applet ($dummy_err)"
    }

    set DATAFLASH_initialized 1
}

#===============================================================================
#  proc DATAFLASH::EraseAll
#-------------------------------------------------------------------------------
proc DATAFLASH::EraseAll {} {
    global target
    global softwareStatusLabelVariable
    variable appletMailboxAddr
    
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]
    
    set softwareStatusLabelVariable "Full Erase in progress ..."
    
    set sizeToErase $GENERIC::memorySize
    set eraseOffset 0
    while {$sizeToErase > 0} {
        puts "-I- \tErasing: [format "0x%X"  $GENERIC::appletBufferSize] bytes at address [format "0x%X" $eraseOffset]"
        # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(erasebuffer) $appletAddrCmd} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet command ($dummy_err)"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $eraseOffset $appletAddrArgv0} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet erase all \n$dummy_err"
        }
        # Launch the applet Jumping to the appletAddr
        if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(erasebuffer)]} dummy_err]} {
            set softwareStatusLabelVariable ""
            error "Applet erase all command has not been launched ($dummy_err)"
        }
        incr eraseOffset $GENERIC::appletBufferSize
        set sizeToErase  [expr $sizeToErase - ($GENERIC::appletBufferSize)]
    }
    set softwareStatusLabelVariable ""
}

#===============================================================================
#  proc DATAFLASH::BinaryPage
#-------------------------------------------------------------------------------
proc DATAFLASH::BinaryPage {} {
    global target
    global commandLineMode
    variable appletMailboxAddr
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]
    
    
    if {$commandLineMode == 0} {
        # Confirm to set the Binary Page
        set returnValue [messageDialg warning.gif "The power-of-2 page size is a onetime programmable configuration register
        and once the device is configured for power-of-2 page size, it cannot be reconfigured again, \n\r Do you want to configure binary page?" "Configure binary page" yesno ]
        if {$returnValue} {
            return -1
        }
    }
    
     # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(binarypage) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }
    
    # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(binarypage)]} dummy_err]} {
        error "Applet set binarypage command has not been launched ($dummy_err)"
    }
    
    if {$commandLineMode == 0} {
        set returnValue [messageDialg warning.gif "Please power down and power up board again \n\r The page for the binary page size can then be programmed " "Configure binary page" ok ]
    }
    puts "-I- Power-0f-2 BInary Page Configured"
    
}


################################################################################
## NAMESPACE SERIALFLASH
################################################################################
namespace eval SERIALFLASH {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
}

#===============================================================================
#  proc SERIALFLASH::Init
#-------------------------------------------------------------------------------
proc SERIALFLASH::Init {sfId} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName

    puts "-I- SERIALFLASH::Init $sfId (trace level : $target(traceLevel))"
    
    # Load the applet to the target
    GENERIC::Init $SERIALFLASH::appletAddr $SERIALFLASH::appletMailboxAddr $SERIALFLASH::appletFileName [list $target(comType) $target(traceLevel) $sfId]
}

#===============================================================================
#  proc SERIALFLASH::EraseAll
#-------------------------------------------------------------------------------
proc SERIALFLASH::EraseAll {} {
    global target
    global softwareStatusLabelVariable
    variable appletMailboxAddr

    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]
    
    set softwareStatusLabelVariable "Full Erase in progress ..."
    
    set sizeToErase $GENERIC::memorySize
    set eraseOffset 0
    while {$sizeToErase > 0} {
        puts "-I- \tErasing one block at address [format "0x%X" $eraseOffset]"
        # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(erasebuffer) $appletAddrCmd} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet command ($dummy_err)"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $eraseOffset $appletAddrArgv0} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet erase all \n$dummy_err"
        }
        # Launch the applet Jumping to the appletAddr
        if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(erasebuffer)]} dummy_err]} {
            set softwareStatusLabelVariable ""
            error "Applet erase all command has not been launched ($dummy_err)"
        }
        # Retrieve how many bytes have been written
        set sizeErased  [TCL_Read_Int $target(handle) $appletAddrArgv0]
        incr eraseOffset $sizeErased 
        set sizeToErase  [expr $sizeToErase - $sizeErased ]
        puts "-I- \t[format "0x%X" $sizeErased] bytes erased"
    }
    set softwareStatusLabelVariable ""
}


################################################################################
## NAMESPACE EEPROM
################################################################################
namespace eval EEPROM {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
}

#===============================================================================
#  proc EEPROM::Init
#-------------------------------------------------------------------------------
proc EEPROM::Init {deviceId} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName

    puts "-I- EEPROM::Init $deviceId (trace level : $target(traceLevel))"
    
    # Load the applet to the target
    GENERIC::Init $EEPROM::appletAddr $EEPROM::appletMailboxAddr $EEPROM::appletFileName [list $target(comType) $target(traceLevel) $deviceId]
}


################################################################################
## NAMESPACE NANDFLASH
################################################################################
namespace eval NANDFLASH {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName

    # Flags to force the erase operaton on block even tagged as bad
    variable scrubErase     0xEA11
    variable normalErase    0x0
}

#===============================================================================
#  proc NANDFLASH::Init
#-------------------------------------------------------------------------------
proc NANDFLASH::Init {} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName

    puts "-I- NANDFLASH::Init (trace level : $target(traceLevel))"

    # Load the applet to the target
    GENERIC::Init $NANDFLASH::appletAddr $NANDFLASH::appletMailboxAddr $NANDFLASH::appletFileName [list $target(comType) $target(traceLevel)]
}


#===============================================================================
#  proc NANDFLASH::BadBlockList
#-------------------------------------------------------------------------------
proc NANDFLASH::BadBlockList {} {
    global target
    variable appletMailboxAddr

    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    # Address where applet write bad block number
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    # Address of the buffer containing bad blocks list
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]


    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(listbadblocks) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }

    # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(listbadblocks)]} dummy_err]} {
        error "Applet List Bad Blocks command has not been launched ($dummy_err)"
    }

    # Read the number of Bad Blocks found
    if {[catch {set nbBadBlocks [TCL_Read_Int $target(handle) $appletAddrArgv0]} dummy_err] } {
        error "Error reading the applet result"
    }

    if { $nbBadBlocks == 0 } {
        puts "-I- No Bad Block found"
        return 1
    } else {
        # Read the address the Bad Blocks list
        if {[catch {set badBlocksListAddr [TCL_Read_Int $target(handle) $appletAddrArgv1]} dummy_err] } {
            error "Error reading the applet result"
        }
        
        puts "-I- Found $nbBadBlocks Bad Blocks :"
        while {$nbBadBlocks > 0} {
            if {[catch {set blockId [TCL_Read_Int $target(handle) $badBlocksListAddr]} dummy_err] } {
                error "Error reading the applet result"
            }

            incr nbBadBlocks -1
            incr badBlocksListAddr 4
            puts "\t Block $blockId"
        }
        return 1
    }

    return 1
}

#===============================================================================
#  proc NANDFLASH::TagBlock
#-------------------------------------------------------------------------------
proc NANDFLASH::TagBlock {blockId type} {   
    global target
    variable appletMailboxAddr
    
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]


    if {$type == "GOOD"} {
        set tag 0xFF
    } else {
        set tag 0x00
    }
    
    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(tagBlock) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }
    
    # Write the block ID in the argument area
    if {[catch {TCL_Write_Int $target(handle) $blockId $appletAddrArgv0} dummy_err] } {
    error "Error Writing Applet Block ID \n$dummy_err"
    }
    
    # Write the tag in the argument area
    if {[catch {TCL_Write_Int $target(handle) $tag $appletAddrArgv1} dummy_err] } {
    error "Error Writing Applet TAG \n$dummy_err"
    }
    
     # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(tagBlock)]} dummy_err]} {
        error "Applet TagBlock command has not been launched ($dummy_err)"
    }
    
    if { $result == 0 } {    	
        return 1
    } else  {
        return -1
    }
}

#===============================================================================
#  proc NANDFLASH::EraseAll
#-------------------------------------------------------------------------------
proc NANDFLASH::BatchEraseAll {param } {
    global target
    global softwareStatusLabelVariable
    variable appletMailboxAddr
    
    set dummy_err 0
    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]

    set softwareStatusLabelVariable "Full Erase in progress ..."
    set eraseBatch 0
    set eraseDone 0
    while {$eraseDone == 0} {
        puts "-I- Erasing  blocks  batch [format "%d" $eraseBatch]"
        # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(batchErase) $appletAddrCmd} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet command ($dummy_err)"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $param $appletAddrArgv0} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet erase all \n$dummy_err"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $eraseBatch $appletAddrArgv1} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet erase all \n$dummy_err"
        }
       
        # Launch the applet Jumping to the appletAddr
        if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(batchErase)]} dummy_err]} {
	        set softwareStatusLabelVariable ""
    	    error "Applet erase all command has not been launched ($dummy_err)"
        }
        set eraseBatch [TCL_Read_Int $target(handle) $appletAddrArgv0]
        if {$eraseBatch == 0} {
            set eraseDone 1
        }
    }
    set softwareStatusLabelVariable ""
    
}


#===============================================================================
#  proc NANDFLASH::BlockErase
#-------------------------------------------------------------------------------
proc NANDFLASH::EraseBlocks {start end} {   
    global target
    variable appletMailboxAddr
    
    set dummy_err 0
    
  # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
   
    if {$start > $end } {
        puts "-E- invalid address"
        return -1
    }
    
    puts "-I- Erase blocks from start 0x$start to 0x$end" 
    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(eraseBlocks) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }
    
    # Write the address offest in the argument area
    if {[catch {TCL_Write_Int $target(handle) $start $appletAddrArgv0} dummy_err] } {
    error "Error Writing Applet Address \n$dummy_err"
    }
    
    # Write the number of blocks in the argument area
    if {[catch {TCL_Write_Int $target(handle) $end $appletAddrArgv1} dummy_err] } {
    error "Error Writing Applet Address \n$dummy_err"
    }
    
    # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(eraseBlocks)]} dummy_err]} {
        error "Applet Block Erase command has not been launched ($dummy_err)"
    }
    if { $result == 0 } {  
        puts "-I- Blocks from address 0x$start to  0x$end erased"
        return 1
    } else  {
        puts "-E- Erase failed"
        return -1
    }  
}


################################################################################
## NAMESPACE NORFLASH
################################################################################
namespace eval NORFLASH {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
}

#===============================================================================
#  proc NORFLASH::Init
#-------------------------------------------------------------------------------
proc NORFLASH::Init {} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
    
    puts "-I- NORFLASH::Init (trace level : $target(traceLevel))"
    
    # Load the applet to the target
    GENERIC::Init $NORFLASH::appletAddr $NORFLASH::appletMailboxAddr $NORFLASH::appletFileName [list $target(comType) $target(traceLevel)]
}

#===============================================================================
#  proc NORFLASH::EraseAll
#-------------------------------------------------------------------------------
proc NORFLASH::EraseAll {} {
    global target
    global softwareStatusLabelVariable
    variable appletMailboxAddr
    
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    set softwareStatusLabelVariable "Full Erase in progress ..."
    
    set sizeToErase $GENERIC::memorySize
    set eraseOffset 0
    while {$sizeToErase > 0} {
    	puts "-I- \tErasing  blocks at address [format "0x%X" $eraseOffset]"
        # Write the Cmd op code in the argument area
        if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(erasebuffer) $appletAddrCmd} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet command ($dummy_err)"
        }
        # Write the buffer address in the argument area
        if {[catch {TCL_Write_Int $target(handle) $eraseOffset $appletAddrArgv0} dummy_err] } {
            set softwareStatusLabelVariable ""
            error "Error Writing Applet erase all \n$dummy_err"
        }
        # Launch the applet Jumping to the appletAddr
        if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(erasebuffer)]} dummy_err]} {
	        set softwareStatusLabelVariable ""
    	    error "Applet erase all command has not been launched ($dummy_err)"
        }
        set bytesErased [TCL_Read_Int $target(handle) $appletAddrArgv0]
        incr eraseOffset $bytesErased
        set sizeToErase  [expr $sizeToErase - $bytesErased]
    }
    set softwareStatusLabelVariable ""
    
}


################################################################################
## NAMESPACE FLASH
################################################################################
namespace eval FLASH {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
    variable flashSize	
    variable flashLockRegionSize
    variable flashNumbersLockBits
    variable ForceUnlockBeforeWrite 0
    variable ForceLockAfterWrite 0
}

#===============================================================================
#  proc FLASH::Init
#-------------------------------------------------------------------------------
proc FLASH::Init { {bank 0} } {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName

    # bank parameter is used for SAM3 devices where two flash memories are at two different base addresses
    GENERIC::Init $FLASH::appletAddr $FLASH::appletMailboxAddr $FLASH::appletFileName [list $target(comType) $target(traceLevel) $bank]
}

#===============================================================================
#  proc FLASH::AskForLockSector
#-------------------------------------------------------------------------------
proc FLASH::AskForLockSector { first_lockregion last_lockregion } {
    global   target
    global   commandLineMode
    variable appletMailboxAddr
    variable ForceLockAfterWrite
    
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]
    
    if {$commandLineMode == 0} {
        # Ask for Lock Sector(s)
        set returnValue [messageDialg warning.gif "Do you want to lock involved lock region(s) ($first_lockregion to $last_lockregion) ?" "Lock region(s) to lock" yesno ]
        if {$returnValue} {
            return -1
        } else {
            set ForceLockAfterWrite 1
        }
    }
    
    if {$ForceLockAfterWrite} {
        # Lock all sectors involved in the write
        for {set i $first_lockregion} {$i <= $last_lockregion } {incr i} {
            # Write the Cmd op code in the argument area
            if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(lock) $appletAddrCmd} dummy_err] } {
                error "Error Writing Applet command ($dummy_err)"
            }   
            # Write the page number in the argument area
            if {[catch {TCL_Write_Int $target(handle) $i $appletAddrArgv0} dummy_err] } {
                error "Error Writing Applet page \n$dummy_err"
            }
            # Launch the applet Jumping to the appletAddr
            if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(lock)]} dummy_err]} {
                error "Applet unlock command has not been launched ($dummy_err)"
            }
            puts "-I- Sector $i locked"
        }
    }

    return 1
}

#===============================================================================
#  proc FLASH::AskForUnlockSector
#-------------------------------------------------------------------------------
proc FLASH::AskForUnlockSector { first_lockregion last_lockregion } {  
    global target
    global commandLineMode
    variable appletMailboxAddr
    variable ForceUnlockBeforeWrite
        
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]


    # Ask for Unlock Sector(s)
    if {$commandLineMode == 0} {
        set returnValue [messageDialg warning.gif "Do you want to unlock involved lock region(s) ($first_lockregion to $last_lockregion) ?" "At least one lock region is locked !" yesno ]
        if {$returnValue} {
            return -1
        } else {
            set ForceUnlockBeforeWrite 1
        }
    } else {
        set ForceUnlockBeforeWrite 1
    }
    
    if {$ForceUnlockBeforeWrite} {
        # Unlock all sectors involved in the write    
        for {set i $first_lockregion} {$i <= $last_lockregion } {incr i} {
            # Write the Cmd op code in the argument area
            if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(unlock) $appletAddrCmd} dummy_err] } {
                error "Error Writing Applet command ($dummy_err)"
            }   
        
            # Write the page number in the argument area
            if {[catch {TCL_Write_Int $target(handle) $i $appletAddrArgv0} dummy_err] } {
                error "Error Writing Applet page \n$dummy_err"
            }
            # Launch the applet Jumping to the appletAddr
            if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(unlock)]} dummy_err]} {
                error "Applet unlock command has not been launched ($dummy_err)"
            }
            puts "-I- Sector $i unlocked"
        }
    }

    return 1
}

#===============================================================================
#  proc FLASH::SendFile
#-------------------------------------------------------------------------------
proc FLASH::SendFile { name addr } {
    variable flashSize  
    variable flashLockRegionSize   
    variable flashNumbersLockBits 
    variable appletBufferAddress

    if { [catch {set f [open $name r]}] } {
        puts "-E- Can't open file $name"
        return -1
    }
    fconfigure $f -translation binary
    
    #First Step check the locked sector 
    set dummy_err 0
    set rewrite 0
    set size [file size $name]
    
    set dest [expr $addr & [expr  $flashSize - 1]]    

    # Compute first and last lock regions
    #set lockRegionSize [expr $flashSize / $flashNumbersLockBits]
    set first_sector [expr $dest / $FLASH::flashLockRegionSize]
    set last_sector [expr [expr [expr $dest + $size] -1 ] / $FLASH::flashLockRegionSize]
    puts " first_sector $first_sector last_sector $last_sector "
    
    if {[catch {GENERIC::Write $dest $size $f 0} dummy_err] } {
        switch $dummy_err {
            0x00000004 {
                # Check for locked lock regions in order to unlock them, return if user refuses
                set returnValue [FLASH::AskForUnlockSector $first_sector $last_sector]
                if {$returnValue == -1} {
                    puts "-E- FLASH::SendFile failed: some lock regions are always locked !"
                    close $f
                    return -1
                } else {set rewrite 1}
            }
            default  {
                puts "-E- Generic::Write returned error ($dummy_err)"
                close $f
                return -1
            }
       }
    }
    if {$rewrite == 1} {
        seek $f 0 start
        if {[catch {GENERIC::Write $dest $size $f 0} dummy_err] } {
            puts "-E- Generic::Write returned error ($dummy_err)"
            close $f
            return -1
        }
    }
    # Lock all sectors involved in the write
    FLASH::AskForLockSector $first_sector $last_sector
    close $f
}

#===============================================================================
#  proc FLASH::receiveFile
#-------------------------------------------------------------------------------
proc FLASH::ReceiveFile { name addr size } {

    variable flashSize  
    set dummy_err 0
    
    set dest [expr $addr & [expr  $flashSize -1 ]]
    if {[catch {GENERIC::ReceiveFile $name $dest $size} dummy_err] } {
        puts "-E- Generic:: receiveFile returned error ($dummy_err)"
    }
}

#===============================================================================
#  proc FLASH::EraseAll
#-------------------------------------------------------------------------------
proc FLASH::EraseAll { } {
    variable flashNumbersLockBits   
    global softwareStatusLabelVariable
    set dummy_err 0
    set reerase 0

    global softwareStatusLabelVariable
    set softwareStatusLabelVariable "Full Erase in progress ..."
    
    if {[catch {GENERIC::EraseAll} dummy_err] } {
        switch $dummy_err {
            0x00000004 {
                # Check for locked lock regions in order to unlock them, return if user refuses
                set returnValue [FLASH::AskForUnlockSector 0 [expr $flashNumbersLockBits -1]]
                if {$returnValue == -1} {
                    set softwareStatusLabelVariable ""
                    puts "-E- Send file failed: some lock regions are always locked !"
                    return -1
                } else {set reerase 1}
            }
            default  {
                puts "-E- Generic::EraseAll returned error ($dummy_err)"
                set softwareStatusLabelVariable ""
                return -1
            }
       }
    }
    if {$reerase == 1} {
        if {[catch {GENERIC::EraseAll} dummy_err] } {
            set softwareStatusLabelVariable ""
            puts "-E- Generic::EraseAll returned error ($dummy_err)"
            return -1
        }
    }
   
    set softwareStatusLabelVariable ""
    return 1
}


#===============================================================================
#  proc FLASH::ScriptGPNMV
#-------------------------------------------------------------------------------
proc FLASH::ScriptGPNMV {index} {   
    global   target
    variable appletMailboxAddr
        
    set dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    # Clear GPNVM : action=0
    # Set GPNVM   : action=1

    switch $index {
        0 {
            set action  1
            set gp 0
        }
        1 {
            set action  0
            set gp 0
        }
        2 {
            set action  1
            set gp 1
        }
        3 {
            set action  0
            set gp 1
        }
        4 {
            set action  1
            set gp 2
        }
        5 {
            set action  0
            set gp 2
        }
        6 {
            set action  1
            set gp 3
        }
        7 {
            set action  0
            set gp 3
        }        
    }
    
    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(gpnvm) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }
    
    # Write the gpnvm action in the argument area
    if {[catch {TCL_Write_Int $target(handle) $action $appletAddrArgv0} dummy_err] } {
    error "Error Writing Applet GPNVM action \n$dummy_err"
    }
    
    # Write the gpnvm bit of nvm in the argument area
    if {[catch {TCL_Write_Int $target(handle) $gp $appletAddrArgv1} dummy_err] } {
    error "Error Writing Applet GPNVM index \n$dummy_err"
    }
    
     # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(gpnvm)]} dummy_err]} {
        error "Applet GPNVM command has not been launched ($dummy_err)"
    }
    
    if { $action == 1 } {
        if { $result != 0 } {    	
            puts "-E- Set GPNVM$gp failed"
            return -1
        } else  {puts "-I- GPNVM$gp set"}
    } else {
        if { $result != 0 } {
            puts "-E- Clear GPNVM$gp failed"
            return -1
        } else  {puts "-I- GPNVM$gp cleared"}
    }

    return 1
}

#===============================================================================
#  proc FLASH::ScriptSetSecurityBit
#-------------------------------------------------------------------------------
proc FLASH::ScriptSetSecurityBit { } {
    global   target
    variable appletMailboxAddr
    set      dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(security) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }
    
    # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(security)]} dummy_err]} {
        error "Applet set security command has not been launched ($dummy_err)"
    }
    puts "-I- Security Bit Set"
}

#===============================================================================
#  proc FLASH::ReadUniqueID
#-------------------------------------------------------------------------------
proc FLASH::ReadUniqueID { } {
    global   target
    variable appletMailboxAddr
    set      dummy_err 0

    # Mailbox is 32 word long (add variable here if you need read/write more data)
    set appletAddrCmd       [expr $appletMailboxAddr]
    set appletAddrStatus    [expr $appletMailboxAddr + 0x04]
    set appletAddrArgv0     [expr $appletMailboxAddr + 0x08]
    set appletAddrArgv1     [expr $appletMailboxAddr + 0x0c]
    set appletAddrArgv2     [expr $appletMailboxAddr + 0x10]
    set appletAddrArgv3     [expr $appletMailboxAddr + 0x14]

    # Init the ping pong algorithm: the buffer is active
    set bufferAddress $GENERIC::appletBufferAddress

    # Write the Cmd op code in the argument area
    if {[catch {TCL_Write_Int $target(handle) $GENERIC::appletCmd(readUniqueID) $appletAddrCmd} dummy_err] } {
        error "Error Writing Applet command ($dummy_err)"
    }
    
    # Write the buffer address in the argument area
    if {[catch {TCL_Write_Int $target(handle) $bufferAddress $appletAddrArgv0} dummy_err] } {
        error "[format "0x%08x" $dummy_err]"
    }

    # Launch the applet Jumping to the appletAddr
    if {[catch {set result [GENERIC::Run $GENERIC::appletCmd(readUniqueID)]} dummy_err]} {
        error "Applet set security command has not been launched ($dummy_err)"
    }

    puts "buffer address [format "0x%08x" $bufferAddress]"

    puts "Unique ID :"

    # Read the page containing the unique ID
    set i 0
    while {$i < [expr 256 / 4]} {
        set addr [expr $bufferAddress + $i * 4]
        # Return the error code returned by the applet
        if {[catch {set data [TCL_Read_Int $target(handle) $addr]} dummy_err] } {
            error "Error reading the buffer containing Unique ID"
        }

        puts [format "\t0x%04x : 0x%08x" [expr $i * 4] $data]
        incr i +1
    }
}

################################################################################
## NAMESPACE SDMMC
################################################################################
namespace eval SDMMC {
    # Following variables are defined in <board>.tcl
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
}

#===============================================================================
#  proc SDMMC::Init
#-------------------------------------------------------------------------------
proc SDMMC::Init {} {
    global target
    variable appletAddr
    variable appletMailboxAddr
    variable appletFileName
    
    puts "-I- SDMMC::Init (trace level : $target(traceLevel))"
    
    # Load the applet to the target
    GENERIC::Init $SDMMC::appletAddr $SDMMC::appletMailboxAddr $SDMMC::appletFileName [list $target(comType) $target(traceLevel)]
}
