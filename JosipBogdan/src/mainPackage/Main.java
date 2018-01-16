package mainPackage;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

public class Main {
	public static void main(String [ ] args) throws IOException
	{
		int diagonal;
		int delete = -10;
		int insert = -10;
		int match = 10;
		int skip = -9;
		
		int globalMax = -1;
		int maxX = -1;
		int maxY = -1;
		
		Path aPath = Paths.get("a.txt");
		Path bPath = Paths.get("b.txt");
		 try {

		      char[] aCharArray = readFile(aPath, Charset.defaultCharset()).toCharArray();

		      char[] bCharArray = readFile(bPath, Charset.defaultCharset()).toCharArray();
		      
		      System.out.println(aCharArray);
		      System.out.println(bCharArray);
		      int a = aCharArray.length;
		      int b = bCharArray.length;
		      //int[][] hMatrix = createMatrix(aCharArray.length, bCharArray.length);
		      
		      int[][] tempMatrix = new int[a][b];
				for(int i=0; i<a; i++){
					for(int j=0; j<b; j++){
						if (i==0 || j==0) {
							tempMatrix[i][j] = 0;
						}else {
							tempMatrix[i][j] = 1;
						}
					}
				}
				
				for (int i=1; i<a; i++) {
					for (int j=1; j<b; j++) {
						if (aCharArray[i] == bCharArray[j]) {
							diagonal = match;
						}else {
							diagonal = skip;
						}
					}
				}
		      
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
