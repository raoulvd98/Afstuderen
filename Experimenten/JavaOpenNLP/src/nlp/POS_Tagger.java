package nlp;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

import opennlp.tools.postag.POSModel;
import opennlp.tools.postag.POSSample;
import opennlp.tools.postag.POSTaggerME;
import opennlp.tools.tokenize.WhitespaceTokenizer;

public class POS_Tagger {
    public String main(String sentence) {
        // loading model
        POSModel model = null;
        try {
            InputStream modelIn = new FileInputStream("./src/nl-pos-maxent.bin");
            model = new POSModel(modelIn);
        } catch (IOException e){
            e.printStackTrace();
        }

        POSTaggerME tagger = new POSTaggerME(model);

        WhitespaceTokenizer whitespaceTokenizer= WhitespaceTokenizer.INSTANCE; 
        String[] tokens = whitespaceTokenizer.tokenize(sentence); 
        String[] tags = tagger.tag(tokens); 

        POSSample sample = new POSSample(tokens, tags); 

        return sample.toString();
    }
}