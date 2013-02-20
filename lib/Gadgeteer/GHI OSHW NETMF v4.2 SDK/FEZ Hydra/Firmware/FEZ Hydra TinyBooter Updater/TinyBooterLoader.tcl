#  ----------------------------------------------------------------------------
#          FEZ Hydra TinyBooter Updater Script
#  ----------------------------------------------------------------------------
#  Copyright (c) 2009, GHI Electronics LLC
#
#  All rights reserved.
#


puts "-I---------------------------------"
puts "-I-   GHI Electronics, FEZ Hydra  -"
puts "-I-   TinyBooter Updater Script   -"
puts "-I---------------------------------"
puts "-I Enable DataFlash"
#DATAFLASH::SelectDataflash AT91C_SPI0_CS0
DATAFLASH::Init 0

#puts "-I Erasing DataFlash"
# The following command will have DataFlash completely erase
DATAFLASH::EraseAll

send_file {DataFlash AT45DB/DCB} "FEZ_HYDRA_TINYBOOTER.bin" 0x8400 0
#compare_file  {DataFlash AT45DB/DCB} "FEZ_HYDRA_TINYBOOTER.bin" 0x8400 0

#send_file {DataFlash AT45DB/DCB} "FEZ_HYDRA_FIRMWARE.bin" 0x84000 0
#compare_file  {DataFlash AT45DB/DCB} "FEZ_HYDRA_FIRMWARE.bin" 0x84000 0

# This line makes "GHI_OSHW_BOOTSTRAP.bin" bootable. Without it, the board will not boot.
GENERIC::SendFile "GHI_OSHW_BOOTSTRAP.bin" 0x0 1


puts "-I---------------------------------"
puts "-I-        Script Completed       -"
puts "-I-     Please Reset the Device   -"
puts "-I---------------------------------"
