prviniz=['A','C','C','G','G']
druginiz=['C','A','G','G']

delete=-10
insert=-10
match=10
skip=-9

globmax=-1
maxx=-1
maxy=-1

prviniz.insert(0,'x')
druginiz.insert(0,'y')
tablica=[]
prazanredak=[]

for j in range(len(druginiz)):
    prazanredak.append(0)
tablica.append(prazanredak)
for i in range(len(prviniz)-1):
    tablica.append([0])

for i in range(1,len(prviniz)):
    for j in range(1,len(druginiz)):
        if(prviniz[i]==druginiz[j]):
            koso=match
        else:koso=skip
        gore=tablica[i-1][j]+insert
        lijevo=tablica[i][j-1]+delete
        gorelijevo=koso+tablica[i-1][j-1]
        value=max(0,gorelijevo,gore,lijevo)
        tablica[i].append(value)
        if value>globmax:
            globmax=value
            maxx=i
            maxy=j
            
def printaj(tablica):
    for i in range(len(tablica)):
        for j in range(len(tablica[i])):
            print(tablica[i][j],end=" ")
        print()

printaj(tablica)
