I use this repository to save all my C# exercises and share them.

Here you can see all the programs description and they functions.

Program.ED: 

exercise: Electronic Devices Management
You are tasked with designing a system to manage a list of electronic devices, including computers, phones, and tablets.
All devices share common actions like turning on, turning off, and displaying basic information but also have unique characteristics.

Requirements:

Create an interface, IDevice, with methods: void TurnOn(), void TurnOff(), and void DisplayInfo().
Develop an abstract base class, ElectronicDevice, implementing IDevice, with common properties (Brand, Model, Price), a 
constructor for initialization, and basic implementations for TurnOn() and TurnOff().
Create derived classes:
Computer: Adds a ProcessorType property.
Phone: Adds an OperatingSystem property.
Tablet: Adds a ScreenSize property.
Each class overrides DisplayInfo() to include its specific properties along with the common ones.
Main Method:

Create a list of devices (List<IDevice>), add 
instances of various types, and invoke the TurnOn, DisplayInfo, and
TurnOff methods for each device.
