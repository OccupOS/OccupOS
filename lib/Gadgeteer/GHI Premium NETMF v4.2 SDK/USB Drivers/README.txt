This USB Driver folder contains multiple drivers. 

GHI_Bootloader_Interface - Used by a GHI board that has to communicate through a bootloader interface, this is not TinyBooter. This  
bootloader will install a virtual serial COM port on the PC.

GHI_NETMF_Interface - Used by every board for deploying applications, debugging applications and for loading the firmware. This is  
installed on the system and Windows will load automatically when a NETMF device is plugged in. In some rare cases, this driver can cause  
BSoD (System Crash). An alternative would be the WinUSB driver below.

GHI_NETMF_WinUsb - This is optional and it is same as GHI_NETMF_Interface but this one uses Microsoft's WinUSB framework. This  
eliminates any system crash but this driver may not function on all operating systems and on virtual machines. It also may not be  
supported on all devices. This page will have more details http://wiki.tinyclr.com/index.php?title=WinUSB_vs_NETMF-USB
