#! /bin/bash
read -p "Input desired viewing frame：" n
ss=$(printf "out_file.%05d" $n)
cp -f ./output/ss/$ss ./
mv $ss frame.ss
./rpg > rpg_log.txt
echo "[Initial] Density and mass："
line=7
head -"$line" rpg_log.txt | tail -1
echo " "
echo "[State at t=$nΔt] "
./rpx > rpx_log.txt << XXG
y
n
frame.ss
0

n

y
XXG
echo "Bulk density："
line=5
head -"$line" rpx_log.txt | tail -1
echo "COM position："
line=6
head -"$line" rpx_log.txt | tail -1
echo "COM velocity："
line=7
head -"$line" rpx_log.txt | tail -1
rm -f rpg_log.txt rpx_log.txt frame.ss