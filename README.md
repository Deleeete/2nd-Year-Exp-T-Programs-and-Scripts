# 2nd-Year-Exp-T-Programs-and-Scripts
Second-year experiment T utils

## Programs
### RocheeeLimit.exe
Usage: The actual performer of the experiment. Its behaviour is defined by the file "setup.par".

The program will automatically step UP a specific parameter in rpg.par, change the dDelta to an appropriate value, and then run the simulation.
All mediate results will be store into "./output/" folder, and when finished, the program will produce an overall csv table with all median of distances data called "all_XX_YY_ZZ.csv", where XX, YY and ZZ are hours, minutes and seconds of the created time of the csv file.

### setup.par
Usage: A file which contains parameters to define the program's behaviour.

Format of all lines: 
[Descrption]=[Value]

For example: 
Number of iteration=100
Pattern string=abcdefg

Note that there should NOT be extra white space character around the "=" sign. The description can be any string since the program just does not care. The program uses line number to identify the meaning of the parameters, the order of the lines matters.

The definitive meaning of the lines are:

Line1: The number of the line that contains the desired stepping parameter. For example, if one needs to change the density parameter in rpg.par, which is located in line 2, then the first line of setup.par should be written as "Any informative text such as TargetLine=2"

Line2: The pattern of changing for that line. Use '$' to indicate where should the stepped parameter be placed. For example, for density, line 2 should be written as something like "Bulk density	$	# in kg/m^3 ". The '$' will be replaced by the changing number during runtime.

Line3: The initial value of the stepping parameter. Can be a decimal. 

Line4: The increment for the target parameter to step every time. 

Line5: The expected number of stepping the target parameter and rerun the simulation. 

Line6: Whether redirect the std error to program output or not. Can be set as "true" or "false". When set true, the error from rpg or rpx programs will show during the process of RocheeeLimit.exe, which will be good for debugging but noisy for normal usage.

Line7: The standard of destruction. All comets with a median of distances that higher than this value will be marked as destructed. When detected the destructed frame, the program RocheeeLimit.exe will automatically calculate the distance and split the data into "./output/csv/out.csv".

## Scripts
### rpx.sh
Usage: rpx is an interactive program, which brings much inconvenience. By changing parameters and running rpx.sh rather than rpx itself, we can avoid inputting same parameters to rpx again and again, as well as the mess after inputting wrong parameters.

The lines between "XXG" are inputs to the rpx. Modify them to get the desired input. For example, changing line 6 to "some_text.ss" will change the rpx input file name from the default "sl9.ss" to "some_text.ss" once and for all.

### draw.sh
Usage: Run ssdraw program which can produce images for all produced ss files. It will move all output rasterised files to "./output/ras/". Since ras file is not supported for most of the software, it is kind of pointless to use this script independently. It is called in other scripts.

### ffmpeg.sh
Usage: Convert all ras files in "./output/ras/" into jpegs in "./output/jpg/", and then a H.264 encoded mp4 video file in "./output/".

### calc.sh
Usage: Run pkdgrav over the produced output files. All output files will then be moved into "./output/ss/".

### run_all_task.sh
Usage: Run all tasks above in the correct order.

### data.sh
Usage: Can help to print the physical properties of the comet at a known time step.

### data_auto.sh
Usage: Can help to print all COM position of the comet between two known time step.

### rpx_helper.sh
Usage: This file is only a helper script for program RocheeeLimit.exe. It is necessary for RocheeeLimit.exe to run properly.

### bt.sh
Usage: Another helper script for RocheeeLimit.exe to called. Necessary.

### c2.sh
Usage: A extended version of calc.sh that not only do the calculation tasks but also do the visualisation tasks. Existing just for convenience.
