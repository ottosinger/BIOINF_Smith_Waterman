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
import java.io.IOException;
import java.io.InputStreamReader;

public class Main {
	public static void main(String [ ] args) throws IOException
	{
		int similarity = 0;
		int delete = -1;
		int insert = -1;
		int match = 1;
		int mismatch = -1;

		
		int maxScore = 0;
		int maxI = 0;
		int maxJ = 0;
		BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
        System.out.print("Enter First Genome File name");
		System.out.println("\n");
        String frst = br.readLine();
        System.out.print("Enter Second Genome File name");
		System.out.println("\n");
        String scnd = br.readLine();
		Path aPath = Paths.get(frst);
		Path bPath = Paths.get(scnd);
		
		
		
		 try {

		      char[] firstGenome = readFile(aPath, Charset.defaultCharset()).toCharArray();

		      char[] secondGenome = readFile(bPath, Charset.defaultCharset()).toCharArray();
		      
		      System.out.println(firstGenome);
		      System.out.println(secondGenome);
		      int a = firstGenome.length;
		      int b = secondGenome.length;
		      
		      int[][] hMatrix = new int[b+1][a+1];
				for(int i=0; i<b+1; i++){ // punjenje prvog reda i stupca sa nulama
					for(int j=0; j<a+1; j++){
						if (i==0 || j==0) {
							hMatrix[i][j] = 0;
						}
					}
				}
				
				for (int i=1; i<b+1; i++) { //kreiranje matrice i racunanje score-a
					for (int j=1; j<a+1; j++) {
						if (firstGenome[j-1] == secondGenome[i-1]) {
							similarity = match;
						}else {
							similarity = mismatch;
						}
						int diagonalValue = hMatrix[i-1][j-1] + similarity;
						int upValue = hMatrix[i-1][j] + insert;
						int downValue = hMatrix[i][j-1] + delete;
						
						int score = Math.max(0, Math.max( diagonalValue,Math.max(upValue, downValue)));//racunanje najvece vrijednosti
						if (score > maxScore) {
							maxScore = score;
							maxI = i-1;
							maxJ = j-1;
						}
						hMatrix[i][j] = score;
					}
				}
				for (int i = 0; i < secondGenome.length; i++) {
					System.out.print(secondGenome[i] + " ");
				}
				System.out.println("\n");
				for(int i = 0; i < b; i++) {
					for (int j = 0; j<a; j++) {
						System.out.print(hMatrix[i][j] + " ");
					}
					System.out.println("\n");
				}
				System.out.println("Score:" + maxScore + "MaxX:" + maxI + "MaxY:" + maxJ);
				
				List<Integer[]> elementList = new ArrayList<>();
				
				
				Integer[] max = new Integer[4];
				int weight[] = {match, mismatch, insert, delete};
				max[0] = maxScore;
				max[1] = maxI;
				max[2] = maxJ;
				max[3] = 10;
				elementList.add(max);
				
				int x = maxI;
				int y = maxJ;
				
				String previousElement = NextMove(hMatrix, x+1, y+1, weight);
				while(previousElement != "stop" && previousElement != "invalid move") {
					if (previousElement == "diagonalMatch" || previousElement == "diagonalMismatch") {
						Integer[] temp = new Integer[4];
						temp[0] = hMatrix[x+1 - 1][y+1 - 1];
						temp[1] = x-1;
						temp[2] = y-1;
						temp[3] = 0;
						
						if (previousElement == "diagonalMatch") {
							temp[3] = 10;
						}else {
							temp[3] = 11;
						}
						elementList.add(temp);
						x--;
						y--;
						
					}else {
						if (previousElement == "up") {
							Integer[] temp = new Integer[4];
							temp[0] = hMatrix[x+1 - 1][y+1];
							temp[1] = x-1;
							temp[2] = y;
							temp[3] = 3;
							elementList.add(temp);
							x--;
						}else {
							if (previousElement == "left") {
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
					previousElement = NextMove(hMatrix,x+1, y+1, weight);
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

		    } catch (IOException e) {
		      System.out.println(e);
		    }
			
		
	}
	
	public static String getAlignStr(List<Integer[]> elementList, char[] firstGenome, char[] secondGenome) {
		String align = "";
		for (Iterator<Integer[]> i = elementList.iterator(); i.hasNext();) {
		    Integer[] temp = i.next();
			System.out.println("prvi " + temp[0] + "drugi " + temp[1] + "treci " + temp[2] + "cetvrti " + temp[3]);

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
	
	static String readFile(Path path, Charset encoding) 
			  throws IOException 
	{
		byte[] encoded = Files.readAllBytes(path);
		return new String(encoded, encoding);
	}
	
	
	
}
