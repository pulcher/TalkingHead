# TalkingHead
This is a creepy talking head with eyes.

# Prereqs:
- .NET 5.0 Framework
- Flite speech synth: http://www.speech.cs.cmu.edu/flite/doc/flite_toc.html
- Raspberry Pi 3B+
- Adafruit Eye Bonnent Kit

# Getting going:
Start with a 'dotnet restore'

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


