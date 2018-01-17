'''def printaj(table):
    for i in range(len(table)):
        for j in range(len(table[i])):
            print(table[i][j],end=" ")
        print()'''
        
def traceback(table,globmax,x,y):
    indeksi=[]
    i=x
    j=y
    while(table[i][j]!=0):
        indeksi.append([i,j])
        if( (table[i-1][j-1]+match==table[i][j]) or  (table[i-1][j-1]+missmatch==table[i][j]) ):
            i-=1
            j-=1
        elif(table[i-1][j]+insert==table[i][j]):
            i=-1
        elif(table[i][j-1]+delete==table[i][j]):
            j=-1
        else:
            print("greska")
            return 0
    indeksi.append([i,j])
    return(indeksi)
#unix read file
import sys
filename = sys.argv[-1]
filename2 = sys.argv[-2]
ii=0
firstrow=[]
with open(filename) as fp:
    for line in fp:
        if ii!=0:
            for jj in range(len(line)):
                if (line[jj] in {'A','C','T','G','U'}):
                    firstrow.append(line[jj])
        ii+=1

ii=0
secondrow=[]
with open(filename2) as fp2:
    for line2 in fp2:
        if ii!=0:
            for jj in range(len(line)):
                if (line[jj] in {'A','C','T','G','U'}):
                    secondrow.append(line[jj])
        ii+=1
sys.exit()
delete=-10
insert=-10
match=10
missmatch=-9

globmax=-1
maxx=-1
maxy=-1

firstrow.insert(0,'x')
secondrow.insert(0,'y')
table=[]
emptyrow=[]

for j in range(len(secondrow)):
    emptyrow.append(0)
table.append(emptyrow)
for i in range(len(firstrow)-1):
    table.append([0])

for i in range(1,len(firstrow)):
    for j in range(1,len(secondrow)):
        if(firstrow[i]==secondrow[j]):
            koso=match
        else:koso=missmatch
        gore=table[i-1][j]+insert
        lijevo=table[i][j-1]+delete
        gorelijevo=koso+table[i-1][j-1]
        value=max(0,gorelijevo,gore,lijevo)
        table[i].append(value)
        if value>globmax:
            globmax=value
            maxx=i
            maxy=j
            
rezz=traceback(table,globmax,maxx,maxy)
prviporedani=[]
drugiporedani=[]
        
print(rezz)
