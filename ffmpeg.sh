#! /bin/bash
rm -f ./output/jpg/*.jpg
ffmpeg -i ./output/ras/out_file.%05d.ras -q:v 2 ./output/jpg/out%05d.jpg
ffmpeg -i ./output/jpg/out%05d.jpg -b:v 16M -vcodec h264 -y ./output/output.mp4