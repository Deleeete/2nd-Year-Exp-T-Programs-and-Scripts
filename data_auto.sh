#! /bin/bash
read -p "Input starting frame：" start
read -p "Input ending frame (inclusive)：" end
for ((n=$start;n<=$end;n++))
do
ss=$(printf "out_file.%05d" $n)
cp -f ./output/ss/$ss ./
mv $ss frame.ss
./rpg > rpg_log.txt
./rpx > rpx_log.txt << XXG
y
n
frame.ss
0

n

y
XXG
echo "[$n]"
line=6
head -"$line" rpx_log.txt | tail -1
rm -f rpg_log.txt rpx_log.txt frame.ssdone
done