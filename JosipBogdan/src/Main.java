import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

public class Main {
	public static void main(String [ ] args) throws IOException
	{
		Path path = Paths.get("C:\\Users\\Bogdan\\Documents\\BIOINF_Smith_Waterman\\JosipBogdan", "a.txt");
		 try {
		      byte[] wikiArray = Files.readAllBytes(path);

		      String wikiString = new String(wikiArray, "ISO-8859-1");
		      System.out.println(wikiString);
		    } catch (IOException e) {
		      System.out.println(e);
		    }
		//String content = readFile(path, StandardCharsets.UTF_8);
			//System.out.print(content);
	}
	
	static String readFile(Path path, Charset encoding) 
			  throws IOException 
	{
		byte[] encoded = Files.readAllBytes(path);
		return new String(encoded, encoding);
	}
}
