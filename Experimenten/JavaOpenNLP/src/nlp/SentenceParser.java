package nlp;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

import opennlp.tools.cmdline.parser.ParserTool;
import opennlp.tools.parser.Parse;
import opennlp.tools.parser.Parser;
import opennlp.tools.parser.ParserFactory;
import opennlp.tools.parser.ParserModel;  
 
public class SentenceParser { 
	public Parse[] main(String sentence) {  
	
		//Loading parser model 
		ParserModel model = null;
		try {
			InputStream modelIn = new FileInputStream("./src/en-parser-chunking.bin");
			model = new ParserModel(modelIn);
		} catch (IOException e) {
			e.printStackTrace();
		}	   
		  
		//Creating a parser 
	  	Parser parser = ParserFactory.create(model); 
	  
		//Parsing the sentence 
		Parse topParses[] = ParserTool.parseLine(sentence, parser, 1); 
	

	  	//for (Parse p : topParses) {
		//	p.show();   
		//}
		return topParses;       
	}
}    