package nlp;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

import opennlp.tools.sentdetect.SentenceDetectorME; 
import opennlp.tools.sentdetect.SentenceModel;  

public class SentenceDetection { 
    public String[] main(String doc) throws Exception { 
        //Loading sentence detector model 
        SentenceModel model = null;
        try {
            InputStream inputStream = new FileInputStream("./src/nl-sent.bin"); 
            model = new SentenceModel(inputStream); 
        } catch (IOException e){
            e.printStackTrace();
        }
        
        //Instantiating the SentenceDetectorME class 
        SentenceDetectorME detector = new SentenceDetectorME(model);  

        //Detecting the sentence
        String sentences[] = detector.sentDetect(doc); 

        return sentences;
    }
}