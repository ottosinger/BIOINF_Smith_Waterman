package mainPackage;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.*;
import java.util.List;
import java.util.Arrays;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;

public class Main {
	static String firstFileName;
	static String secondFileName;
	public static char[] input(String str) throws IOException{
		BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
        System.out.print("Enter" + " " + str + " " + "Genome File name");
		System.out.println("\n");
        String frst = br.readLine();
		Path path = Paths.get(frst);
		if (str == "First") {
			firstFileName = frst;
		}else {
			secondFileName = frst;
		}
		try {
		      char[] genome = readFile(path, Charset.defaultCharset()).toCharArray();
		      return genome;
		} catch (IOException e) {
		      System.out.println(e);
		     return input(str);
		    }

	}
	
	public static void main(String [ ] args) throws IOException
	{
		int similar = 0;
		int delete = -1;
		int insert = -1;
		int match = 1;
		int mismatch = -1;
		int maxScore = 0;
		int maxX = 0;
		int maxY = 0;
			  
		      char[] firstGenome = input("First");
		      String tempStringOne = new String(firstGenome);
		      tempStringOne.replaceAll("\\s+",""); //uklanjanje nepotrebnih znakova
		      firstGenome = tempStringOne.toCharArray();

		      char[] secondGenome = input("Second");
		      String tempStringTwo = new String(secondGenome);
		      tempStringTwo.replaceAll("\\s+",""); //uklanjanje nepotrebnih znakova
		      secondGenome = tempStringTwo.toCharArray();
		      
		      System.out.println("");
		      System.out.println(firstGenome);
		      System.out.println(secondGenome);
		      int a = firstGenome.length;
		      int b = secondGenome.length;
		      System.out.println("");

		      int[][] hMatrix = new int[b+1][a+1];
				for(int i=0; i<b+1; i++){ 
					for(int j=0; j<a+1; j++){
						if (i==0 || j==0) {
							hMatrix[i][j] = 0; // punjenje prvog reda i stupca sa nulama
						}
					}
				}
				
				for (int i=1; i<b+1; i++) { //kreiranje matrice i racunanje score-a po jednadzbabama 6 i 7
					for (int j=1; j<a+1; j++) {
						if (firstGenome[j-1] == secondGenome[i-1]) { // ako je doslo do podudaranja
							similar = match;
						}else {
							similar = mismatch;
						}
						int diagonalValue = hMatrix[i-1][j-1] + similar; //racunanje diagonalne vrijednosti
						int upValue = hMatrix[i-1][j] + insert;
						int downValue = hMatrix[i][j-1] + delete;
						
						int score = Math.max(0, Math.max( diagonalValue,Math.max(upValue, downValue)));//racunanje najvece vrijednosti
						if (score > maxScore) {
							maxScore = score;
							maxX = i-1;
							maxY = j-1;
						}
						hMatrix[i][j] = score;
					}
				}
				System.out.print("  P ");

				for (int i = 0; i < firstGenome.length; i++) {
					System.out.print(firstGenome[i] + " ");
				}
				System.out.println("\n");
				System.out.print("S ");

				for(int i = 0; i < b+1; i++) { //Ispisivanje matrice
					if (i!=0 && i != b+1) {
						System.out.print(secondGenome[i-1] + " ");
					}
					for (int j = 0; j<a+1; j++) {
						System.out.print(hMatrix[i][j] + " ");
					}
					System.out.println("\n");
				}
				System.out.println("Score:" + maxScore + "MaxX:" + maxX + "MaxY:" + maxY);
				
				List<Integer[]> elementList = new ArrayList<>();
				
				Integer[] max = new Integer[4];
				int weight[] = {match, mismatch, insert, delete};
				max[0] = maxScore;
				max[1] = maxX;
				max[2] = maxY;
				max[3] = 10;
				elementList.add(max);
				
				int x = maxX;
				int y = maxY;
				
				String prevElement = NextMove(hMatrix, x+1, y+1, weight);
				while(prevElement != "stop" && prevElement != "invalid move") {
					if (prevElement == "diagonalMatch" || prevElement == "diagonalMismatch") {
						Integer[] temp = new Integer[4];
						temp[0] = hMatrix[x+1 - 1][y+1 - 1];
						temp[1] = x-1;
						temp[2] = y-1;
						temp[3] = 0;
						
						if (prevElement == "diagonalMatch") {
							temp[3] = 10;
						}else {
							temp[3] = 11;
						}
						elementList.add(temp);
						x--;
						y--;
						
					}else {
						if (prevElement == "up") {
							Integer[] temp = new Integer[4];
							temp[0] = hMatrix[x+1 - 1][y+1];
							temp[1] = x-1;
							temp[2] = y;
							temp[3] = 3;
							elementList.add(temp);
							x--;
						}else {
							if (prevElement == "left") {
								Integer[] temp = new Integer[4];
								temp[0] = hMatrix[x+1][y+1 - 1];
								temp[1] = x;
								temp[2] = y-1;
								temp[3] = 2;
								elementList.add(temp);
								y--;
							}else {
								break;
							}
						}
					}
					prevElement = NextMove(hMatrix,x+1, y+1, weight);
				}
				
				if (firstGenome[elementList.get(elementList.size() - 1)[2]] != secondGenome[elementList.get(elementList.size() - 1)[1]]) {
					elementList.remove(elementList.size() - 1);
				}
				
				Collections.reverse(elementList);
				
				int firstGenomeAlignStart = elementList.get(0)[2];
				int secondGenomeAlingStart = elementList.get(0)[2];
				
				int firstGenomeAlignEnd = elementList.get(elementList.size() - 1)[2];
				int secondGenomeAlignEnd = elementList.get(elementList.size() - 1)[1];
				
				String alignStr = getAlignStr(elementList, firstGenome, secondGenome); 
				
				System.out.print(alignStr);
				String str = "##maf version=1 scoring=" + firstFileName;
				str += System.lineSeparator();
				str += "a score=" + maxScore;
				str += System.lineSeparator();
				str += "s " + secondFileName + " " + secondGenomeAlingStart + " " + b + " " + secondGenomeAlignEnd + " " + alignStr; 
				write(str);
	}
	
	public static void write(String str) // stvaranje tekstualnog dokumenta sa konacnim rezultatima
			  throws IOException {
			    PrintWriter writer = new PrintWriter("result.txt");
			    writer.write(str);
			     
			    writer.close();
			}

	public static String getAlignStr(List<Integer[]> elementList, char[] firstGenome, char[] secondGenome) {
		String align = "";
		for (Iterator<Integer[]> i = elementList.iterator(); i.hasNext();) {
		    Integer[] temp = i.next();
		    if (temp[3] == 10) { //match
		    	int coordinateY = temp[1];
		    	align += secondGenome[coordinateY];
		    }else {
		    	if (temp[3] == 11) {//mismatch
		    		int coordianteY = temp[1] + 1;
		    		align += secondGenome[coordianteY];
		    	}else {
		    		if(temp[3] == 2) { //deletion
		    			align += ".";
		    		}else {
		    			if (temp[3] == 3) { //insertion
		    				align += "-";
		    			}else {
		    				System.out.print("error\n");
		    			}
		    		}
		    	}
		    }
		}
		return align;
	}
	
	public static String NextMove(int Matrix[][], int x, int y, int weight[]) {
		int match = weight[0];
		int mismatch = weight[1];
		int insertion = weight[2];
		int deletion = weight[3];
		
		int diagonal = Matrix[x - 1][ y - 1];
		int up = Matrix[x - 1][y]; 
		int left = Matrix[x][y-1];
		
		int current = Matrix[x][y];
		
		if (diagonal + match == current || diagonal + mismatch == current) {
			if (diagonal != 0){
				if (diagonal + match == current) {
					return "diagonalMatch";
				}else {
					return "diagonalMismatch";
				}
			}else {
				return "stop";
			}
			
		}else {
			if (up + insertion == current) {
				if (up != 0) {
					return "up";
				}else {
					return "stop";
				}
			}else {
				if (left + deletion == current) {
					if (up != 0) {
						return "left";
					}else {
						return "stop";
					}
				}else {
					return "invalid move";
				}
			}
		}
	}
	
	static String readFile(Path path, Charset encoding)  //citanje iz datoteke
			  throws IOException 
	{
		byte[] encoded = Files.readAllBytes(path);
		return new String(encoded, encoding);
	}
	
	
	
}
