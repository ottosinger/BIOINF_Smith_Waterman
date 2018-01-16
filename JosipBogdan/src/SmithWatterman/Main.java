package SmithWatterman;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

public class Main {
	public static void main(String [ ] args) throws IOException
	{

		Path a = Paths.get("a.txt");
		Path b = Paths.get("b.txt");
		 try {

		      char[] aCharArray = readFile(a, Charset.defaultCharset()).toCharArray();

		      char[] bCharArray = readFile(b, Charset.defaultCharset()).toCharArray();
		      
		      System.out.println(aCharArray);
		      System.out.println(bCharArray);
		      
		      int[][] hMatrix = createMatrix(aCharArray.length, bCharArray.length);
		      
		      for(int i=0; i<aCharArray.length; i++){
					for(int j=0; j<bCharArray.length; j++){
					      System.out.println(hMatrix[i][j]);
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
	
	
	static int[][] createMatrix(int a, int b) 
	{
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
		return tempMatrix;
	}
}
