package mainPackage;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

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
				
		    } catch (IOException e) {
		      System.out.println(e);
		    }
			
		
	}
	
	static String readFile(Path path, Charset encoding) 
			  throws IOException 
	{
		byte[] encoded = Files.readAllBytes(path);
		return new String(encoded, encoding);
	}
	
	
	
}
