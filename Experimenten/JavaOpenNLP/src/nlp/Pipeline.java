package nlp;

import java.util.ArrayList;
import java.util.List;

// http://opennlp.sourceforge.net/models-1.5/

public class Pipeline {
    public static void main(String[] args) {
        String Doc = "Deze prijs is exclusief btw";
        new Pipeline();
        Pipeline.setPipeline(Doc);
    }

    public static void setPipeline(String Doc) {
        // Split doc in different sentences
        SentenceDetection sentenceDetection = new SentenceDetection();
        String[] sentences = null;
        try {
            sentences = sentenceDetection.main(Doc);
        } catch (Exception e) {
            e.printStackTrace();
        }

        // Tagger: Assign tags to each word in a sentence
        POS_Tagger tagger = new POS_Tagger();
        List<String> taggs = new ArrayList<String>();
        for(String sent : sentences){
            try {
                var tagged_sentence = tagger.main(sent);
                taggs.add(tagged_sentence);
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        //Tokenize
        Tokenizer tokenizer = new Tokenizer();
        List<String[]> tokens = new ArrayList<>();
        for (int i = 0; i < taggs.size(); i++){
            String[] x = tokenizer.main(taggs.get(i));
            tokens.add(x);
        }

        boolean wrong_btw = false;
        for (int i = 0; i < tokens.size(); i++){
            for (int y = 0; y < tokens.get(i).length; y++){
                // System.out.println(tokens.get(i)[y]);
                if (tokens.get(i)[y].contains("btw") || tokens.get(i)[y].contains("b.t.w.")){
                    btw_sentence sentence_to_check = new btw_sentence(tokens.get(i));
                    sentence_to_check.split();
                    
                    if(sentence_to_check.chech_negative()){
                        wrong_btw = true;
                    }
                }
            }
        }
        System.out.println(Doc);
        if (wrong_btw == false){
            System.out.println("good_btw");
        } else {
            System.out.println("wrong_btw");
        }
    }
}