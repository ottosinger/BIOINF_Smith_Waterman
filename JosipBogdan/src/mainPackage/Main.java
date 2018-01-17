package mainPackage;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.*;
import java.util.List;
import java.util.Arrays;

public class Main {
	public static void main(String [ ] args) throws IOException
	{
		int similarity = 0;
		int delete = -10;
		int insert = -10;
		int match = 10;
		int mismatch = -9;

		
		int maxScore = 0;
		int maxI = 0;
		int maxJ = 0;
		
		Path aPath = Paths.get("a.txt");
		Path bPath = Paths.get("b.txt");
		 try {

		      char[] firstGenome = readFile(aPath, Charset.defaultCharset()).toCharArray();

		      char[] secondGenome = readFile(bPath, Charset.defaultCharset()).toCharArray();
		      
		      System.out.println(firstGenome);
		      System.out.println(secondGenome);
		      int a = firstGenome.length;
		      int b = secondGenome.length;
		      //int[][] hMatrix = createMatrix(aCharArray.length, bCharArray.length);
		      
		      int[][] hMatrix = new int[a+1][b+1];
				for(int i=0; i<a; i++){
					for(int j=0; j<b; j++){
						if (i==0 || j==0) {
							hMatrix[i][j] = 0;
						}
					}
				}
				
				for (int i=1; i<a+1; i++) {
					for (int j=1; j<b+1; j++) {
						if (firstGenome[i-1] == secondGenome[j-1]) {
							similarity = match;
						}else {
							similarity = mismatch;
						}
						int diagonalValue = hMatrix[i-1][j-1] + similarity;
						int upValue = hMatrix[i-1][j] + insert;
						int downValue = hMatrix[i][j-1] + delete;
						
						int score = Math.max(0, Math.max( diagonalValue,Math.max(upValue, downValue)));
						if (score > maxScore) {
							maxScore = score;
							maxI = i-1;
							maxJ = j-1;
						}
						hMatrix[i][j] = score;
					}
				}
				
				System.out.println("Score:" + maxScore + "MaxX:" + maxI + "MaxY:" + maxJ);
				
				List<Integer[]> elementList = new ArrayList<>();
				
				
				Integer[] max = new Integer[3];
				int weight[] = {match, mismatch, insert, delete};
				max[0] = maxScore;
				max[1] = maxI;
				max[2] = maxJ;
				elementList.add(max);
				
				int x = maxI;
				int y = maxJ;
				
				String previousElement = NextMove(hMatrix, x+1, y+1, weight);
				//nextmove
				while(previousElement != "stop" && previousElement != "invalid move") {
					if (previousElement == "diagonalMatch" || previousElement == "diagonalMismatch") {
						Integer[] temp = new Integer[4];
						temp[0] = hMatrix[x+1 - 1][y+1 - 1];
						temp[1] = x-1;
						temp[2] = y-1;
						temp[3] = 0;
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
			System.out.println("prvi" + temp[0] + "drugi" + temp[1] + "treci" + temp[2]);

		    if (temp[3] == 10) {
		    	int coordinateY = temp[1];
		    	align += secondGenome[coordinateY];
		    }else {
		    	if (temp[3] == 11) {
		    		int coordianteY = temp[1] + 1;
		    		align += secondGenome[coordianteY];
		    	}else {
		    		if(temp[3] == 2) {
		    			align += ".";
		    		}else {
		    			if (temp[3] == 3) {
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
