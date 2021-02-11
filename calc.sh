#! /bin/bash
mkdir ./output/
mkdir ./output/ss/
rm -f ./output/ss/*.?????
./pkdgrav ss.par
mv *.????? ./output/ss