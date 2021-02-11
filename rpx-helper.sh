#! /bin/bash
./rpx > rpx_log.txt << XXG
y
n
temp.ss
0

n

y
XXG
line=6
head -"$line" rpx_log.txt | tail -1
rm -f rpx_log.txt temp.ss