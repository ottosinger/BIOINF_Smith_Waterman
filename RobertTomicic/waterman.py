import sys

def writeable(table,firstrow,secondrow):#writes the table values
	sys.stdout.write("   ")
	for j in range(len(secondrow)):
		sys.stdout.write('{0: <3}'.format(str(secondrow[j])) )
	print('')
	sys.stdout.flush
	for i in range(len(table)):
		sys.stdout.write('{0: <3}'.format(str(firstrow[i])) )
		for j in range(len(table[i])):
			sys.stdout.write('{0: <3}'.format(str(table[i][j])) )
		sys.stdout.flush
        	print('')

def traceback(table,globmax,x,y,row,row2):#main traceback function determining path from max value
    result=[]
    i=x
    j=y
    while(table[i][j]!=0):
	topValue=0
	leftValue=0
	topLeftValue=0
	# 4 IFs to determine existsing paths
        if( (table[i-1][j-1]+match==table[i][j])and(row[i]==row2[j]) ):#match path
		topLeftValue=table[i-1][j-1];
	if( (table[i-1][j-1]+missmatch==table[i][j])and(row[i]!=row2[j]) ):#missmatch path
		topLeftValue=table[i-1][j-1];
        if(table[i-1][j]+insert==table[i][j]):#top path
	    topValue=table[i-1][j];
        if(table[i][j-1]+delete==table[i][j]):#left path
	    leftValue=table[i][j-1]
	# 2 IFs to determine the real path
	if( (topLeftValue >= leftValue) and (topLeftValue >= topValue) ): #match/missmatch path chosen
		result.append(row[i])
		i-=1
		j-=1
		continue
	elif (topValue>=leftValue):	#top path chosen
		result.append(row[i])
		i-=1
		continue
	else:				#left path chosen
		result.append('-')
		j-=1
	
    result.reverse()
    return result

#unix read file
filename = sys.argv[-1]
ii=0
firstrow=[]
with open(filename) as fp:
    for line in fp:
        if ii!=0:
            for jj in range(len(line)):
                if (line[jj] in {'A','C','T','G','U'}):
                    firstrow.append(line[jj])
	else:
	    name=line.split(' ')
	    name=name[0][1:]	    
            ii+=1
filenamee = sys.argv[-2]
ii=0
secondrow=[]
with open(filenamee) as fp2:
    for line2 in fp2:
        if ii!=0:
            for jj in range(len(line2)):
                if (line2[jj] in {'A','C','T','G','U'}):
                    secondrow.append(line2[jj])
	else:
	    name2=line2.split(' ')
	    name2=name2[0][1:]
            ii+=1


#fixed algorythm values
delete=-10
insert=-10
match=10
missmatch=-9

#initialise max tracker
globmax=-1
maxx=-1
maxy=-1

#fill table
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
        if( (value>globmax)and (i+j>maxx+maxy) ):
		globmax=value
		maxx=i
		maxy=j
#start traceback on the table         
rezz=traceback(table,globmax,maxx,maxy,firstrow,secondrow)
starting=maxx+1-len(rezz)
rezz="".join(rezz)
#writeable(table,firstrow,secondrow)

#print results
print("##maf version=1")
print("# "+name2)
print("a score="+str(globmax)+".0")
print("s "+name+" "+str(starting)+" "+str(len(rezz))+" + "+str(len(firstrow))+" "+str(rezz))
sys.exit()
