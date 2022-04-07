# TalkingHead
This is a creepy talking head with eyes.

## Requirements
- Visual Studio Community, Visual Studio Code, or your favorite text editor that understands C#.
- Minimum Raspberry Pi 3B+ with Raspberry Pi OS 32bit installed.  
- .NET 6 installed on the RPI.  See the .NET Setup section below
- [Adafruit Animated Eyes](https://www.adafruit.com/product/3813)
- Joystick Module
- [Adafruit lighted arcade button](https://www.adafruit.com/product/3487)
- Power supply
- Speakers or Headphones
- Some PLA if your printing all the models.

## .NET Installation

## Install the needed support packages
```
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```

## Install the .NET SDK
You may find running the scripts the best way todo make this happen.  Checkout the [Microsoft Docs on the script.](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script)

Here is the command I used to install the dotnet SDK in the main location for all users:
```
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 6.0.2xx --install-dir /usr/share/dotnet
```
Then add the following to the end of the PATH variable in either your ~/.bashrc file or for the system wide setting use /etc/environment.

Example:
```
PATH="/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/usr/games:/usr/local/gam
es:/snap/bin:/usr/share/dotnet"
```

Source your .bashrc, or exit and re-logon.  You should not be able to execute the command __dotnet__ and get some output.

## Install the code
Change to your favorite directory.  For me, that is __~/repos__ and enter the following:

```
 git clone https://github.com/pulcher/TalkingHead.git
```
## Setup your own User-secrets
This project needs a view api keys.  Inorder to keep everyone safe, we are using User-Secrets.  

You can find out how to set them up for your repos at [User-Secrests](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows)

## Running the service
1. dotnet restore
- May need to update all the packages. The Scott Hanselman blog on [dotnet-update](https://www.hanselman.com/blog/your-dotnet-outdated-is-outdated-update-and-help-keep-your-net-projects-up-to-date) or head over the [dont-netupdate repositor](https://github.com/dotnet-outdated/dotnet-outdated), give them a star and check them out.















# JUNK from the before times....

# Prereqs:
- .NET 5.0 Framework
- - Need to move to .NET 6
- Azure Cognitive Services
- (optional)Flite speech synth: http://www.speech.cs.cmu.edu/flite/doc/flite_toc.html
- Raspberry Pi 3B+
- Adafruit Eye Bonnent Kit

# Getting going:
Start with a 'dotnet restore'

# Inspirational Quotes:
- Author: Brandi Mummery
- Repository: https://github.com/bmumz/inspirational-quotes-api

# Dev steps
* mkdir Magic8HeadService
* cd Magic8HeadService
* dotnet new worker


## Junk for building and running:
* mkdir Magic8HeadService
*   482  cd Magic8HeadService
*   483  dotnet new worker
*   484  dir
*   485  dotnet run
*   486  dotnet restore
*   487  sudo mkdir /srv/Magic8HeadService
*   488  sudo chown pi /srv/Magic8HeadService/
*   489  dotnet publish -c Release -o /srv/Magic8HeadService/
*   490  /srv/Magic8HeadService/Magic8HeadService
*   491  date
*   492  env
*   493  sudo cp Magic8HeadService.service /etc/systemd/system/
*   494  sudo systemctl daemon-reload
*   495  sudo systemctl start Magic8HeadService
*   496  systemctl status
*   497  sudo journalctl -u Magic8HeadService
*   498  sudo systemctl stop Magic8HeadService
*   499  sudo journalctl -u Magic8HeadService
*   500  sudo cp Magic8HeadService.service /etc/systemd/system/
*   501  sudo systemctl daemon-reload
*   502  sudo systemctl restart Magic8HeadService
*   503  sudo journalctl -u Magic8HeadService
*   504  sudo systemctl restart Magic8HeadService
*   505  sudo journalctl -u Magic8HeadService
*   506  sudo systemctl enable Magic8HeadService
*   507  cd /etc/systemd/system/
*   508  ls
*   509  cd multi-user.target.wants/
*   510  dir
*   511  more Magic8HeadService.service
*   512  cd ..
*   513  cd
*   514  cd repos/TalkingHead/Magic8HeadService/

# To adjust the volume from the command line
apt-get install alsa-utils

GET volume: "amixer -M sget Headphone"
SET volume: "amixer -q -M sset Headphone 50%"


# Stuff for linking:

https://github.com/kritzware/twitch-bot


