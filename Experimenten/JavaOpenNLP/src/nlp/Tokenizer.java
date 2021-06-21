package nlp;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

import opennlp.tools.tokenize.TokenizerME;
import opennlp.tools.tokenize.TokenizerModel;

public class Tokenizer {
    public String[] main(String sentence) {      
        //Loading Tokenizer model 
		TokenizerModel model = null;
		try {
			InputStream modelIn = new FileInputStream("./src/nl-token.bin");
			model = new TokenizerModel(modelIn);
		} catch (IOException e) {
			e.printStackTrace();
		}        
        //Instantiating the TokenizerME class 
        TokenizerME tokenizer = new TokenizerME(model); 
        
        //Tokenizing the given raw text 
        String tokens[] = tokenizer.tokenize(sentence);       
            
        return tokens;
	}
}    