import sys
def printaj(table):
    for i in range(len(table)):
        for j in range(len(table[i])):
		sys.stdout.write(str(table[i][j])+" ")
	sys.stdout.flush()
        print()
        
def traceback(table,globmax,x,y):
    indexes=[]
    i=x
    j=y
    while(table[i][j]!=0):
        indexes.append([i,j])
        if( (table[i-1][j-1]+match==table[i][j]) or  (table[i-1][j-1]+missmatch==table[i][j]) ):
            i-=1
            j-=1
        elif(table[i-1][j]+insert==table[i][j]):
            i=-1
        elif(table[i][j-1]+delete==table[i][j]):
            j=-1
        else:
            print("error")
            return 0
    indexes.append([i,j])
    return(indexes)

#unix read file
print(sys.argv)
filename = sys.argv[-1]
ii=0
firstrow=[]
with open(filename) as fp:
    for line in fp:
        if ii!=0:
            for jj in range(len(line)):
                if (line[jj] in {'A','C','T','G','U'}):
                    firstrow.append(line[jj])
        ii+=1

print(filename+" has")
print(firstrow)

print("-----------------")

filenamee = sys.argv[-2]
ii=0
secondrow=[]
with open(filenamee) as fp2:
    for line2 in fp2:
        if ii!=0:
            for jj in range(len(line2)):
                if (line2[jj] in {'A','C','T','G','U'}):
                    secondrow.append(line2[jj])
        ii+=1
print(filenamee+" has")
print(secondrow)


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
            detour=match
        else:detour=missmatch
        up=table[i-1][j]+insert
        left=table[i][j-1]+delete
        upleft=detour+table[i-1][j-1]
        value=max(0,upleft,up,left)
        table[i].append(value)
        if value>globmax:
            globmax=value
            maxx=i
            maxy=j
            
rezz=traceback(table,globmax,maxx,maxy)
prviporedani=[]
drugiporedani=[]    
printaj(table)
print(rezz)
sys.exit()
