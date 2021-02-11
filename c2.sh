#! /bin/bash

./pkdgrav ss.par
mkdir ./output/ss/
mv *.????? ./output/ss
./draw.sh
./ffmpeg.sh