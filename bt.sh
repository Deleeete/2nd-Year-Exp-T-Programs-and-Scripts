#! /bin/bash
./ss2bt ./output/ss/out_file.?????
mkdir ./output/bt
rm -f ./output/bt/out_file.?????.bt
mv out_file.?????.bt ./output/bt/