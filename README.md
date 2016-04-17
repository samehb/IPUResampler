# IPUResampler
## Introduction
IPUResampler allows you to downsize your IPS/IPU videos in order to save size on your media. It utilizes IPUDecoder, ps2str, and ReJig . ps2str.exe is part of the PS2 SDK. So, it will not be included.

## Usage
1. Compile [IPUDecoder](https://github.com/samehb/IPUDecoder) and this project and place the two executable in a single folder of your choice. 
2. Download [ReJig](http://www.videohelp.com/software/ReJig) and place the exe into the folder from step 1.
3. Place the ps2str.exe (from your PS2 SDK) into the folder from step 1.
4. Place the IPS/IPU files you want resampled into the folder from step 1. The executables and the IPS/IPU files must be in the same folder.
5. Run IPUResampler and drag and drop the files you want to resample into the IPUResampler box. Note that the box only accepts IPS and IPU files.
6. Click Process, then wait for the message to the bottom to show. The statusbar tells you whether or not the files were resampled successfully.

If you receive an error message, that means you did not follow the steps above correctly. Please reread the steps. Also, note that the processing might take a while depending on file sizes. So, wait for the statusbar message to show. That is when you know, the tool has finished (because the project does not have multithreading support).

## Copyright
This project utilizes IPUDecoder, ps2str, and ReJig. Feel free to use the project as you please for non-commercial uses.

## Links
[Blog](http://sres.tumblr.com/)