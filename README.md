# GUI-SCRPY

A modern GUI-based Android Screen Mirroring & Control application built using **C#**, inspired by the power of **Scrcpy**. The project focuses on providing an easy-to-use desktop interface for mirroring and controlling Android devices over **USB** and **WiFi**, while integrating additional utility features for productivity and device management.

---

# Features

## Android Screen Mirroring

* Real-time Android screen mirroring on PC
* Low-latency display for smooth interaction
* Supports high-resolution streaming

## Device Control

* Control Android devices directly from desktop
* Mouse and keyboard interaction support
* Easy navigation and multitasking

## USB & WiFi Connectivity

* Connect devices using USB debugging
* Wireless connection support over WiFi
* Dynamic connection status monitoring

## Webcam Integration

* Webcam support integrated into the application
* Useful for communication, recording, and monitoring purposes

## Battery Monitoring

* Displays Android device battery status
* Helps users monitor charging and battery usage

## File Transfer Support

* Send and receive files between PC and Android device
* Simplifies quick file sharing

## Dynamic Profile Creation

* Create and manage custom device profiles
* Store preferred settings for faster access

## QR Code Scanner

* Integrated QR scanner functionality
* Quick access to links, authentication, and information sharing

## User-Friendly Interface

* GUI designed with focus on simplicity and usability
* Easy navigation for beginners and advanced users
* Clean desktop experience compared to command-line tools

---

# Tech Stack

## Frontend / GUI

* **C# Windows Forms / WPF**

## Backend & Core Tools

* **Scrcpy**
* **ADB (Android Debug Bridge)**

## Additional Technologies

* USB Device Communication
* Socket/WiFi Communication
* Webcam APIs
* QR Code Processing Libraries

---

# Project Motivation

The project was developed to simplify Android screen mirroring and control by creating a graphical interface over Scrcpy. While Scrcpy is extremely powerful, it mainly operates through command-line execution, which can be difficult for beginners.

The aim of this project is to:

* Make Android mirroring more accessible
* Improve usability through GUI interaction
* Add productivity-focused features beyond standard mirroring
* Learn advanced desktop development using C#
* Explore Android-PC communication systems

---

# How It Works

1. The application detects connected Android devices using **ADB**.
2. Once the device is connected through USB or WiFi, the software establishes communication.
3. Scrcpy handles the screen streaming and control functionality.
4. The GUI acts as a management layer for:

   * Device selection
   * Connection handling
   * Status monitoring
   * Additional features like webcam, battery, and file transfer
5. Commands are executed in the background while the user interacts with a simple graphical interface.

---

# Learning Outcomes

This project helped in understanding:

* C# desktop application development
* Android Debug Bridge (ADB)
* Process handling in C#
* Device communication protocols
* GUI/UX design principles
* Multi-feature desktop software architecture
* Real-time streaming integration

---

# Screenshots

> Add your application screenshots here.
> GuiScrcpy/scrw.png
Example:

```md
![Home Screen](images/home.png)
![Connection Window](images/connect.png)
![Mirror View](images/mirror.png)
```

---

# Project Structure

```bash
GUI-SCRPY/
│
├── GUI/
├── Core/
├── ADB/
├── Scrcpy/
├── Assets/
├── Profiles/
├── QRScanner/
├── Webcam/
└── README.md
```

---

# Installation

## Prerequisites

* Windows OS
* Android Device with USB Debugging Enabled
* ADB Installed
* Scrcpy Installed
* .NET Framework / .NET SDK

## Steps

1. Clone the repository

```bash
git clone https://github.com/your-username/GUI-SCRPY.git
```

2. Open the project in Visual Studio

3. Build and run the application

4. Connect your Android device

5. Start mirroring

---

# Future Improvements

* Multi-device support
* Screen recording functionality
* Audio streaming support
* Better UI animations and themes
* Cross-platform support
* Cloud synchronization
* Performance optimization

---

# Challenges Faced

* Handling stable USB and WiFi connections
* Managing real-time device communication
* Synchronizing GUI with backend processes
* Integrating multiple features into one application
* Improving UI responsiveness and usability

---

# Inspiration

This project is inspired by:

* The simplicity and power of **Scrcpy**
* Interest in Android-PC interaction systems
* Desire to create user-friendly developer tools
* Learning advanced software integration concepts

---

# Contribution

Contributions, ideas, and improvements are welcome.

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

---

# License

This project is developed for educational and learning purposes.

---

# Author

**Chetan Choudhary**

B.Tech CSE Student | Unity Developer | C++ & C# Enthusiast

---

# Support

If you like this project, consider giving it a ⭐ on GitHub.
