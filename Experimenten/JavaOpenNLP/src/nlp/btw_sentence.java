package nlp;

import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;


public class btw_sentence {
    String[] sentence;
    List<Tuple<String, String>> Sentence_type;
	public btw_sentence(String[] Sentence) {
        sentence = Sentence;  // Set the initial value for the class attribute x
        Sentence_type =  new ArrayList<Tuple<String,String>>();
    }

    public void split() {      
        for(int i = 0; i < sentence.length; i++) {
            String single_word = (String)Array.get(sentence, i);
            if (single_word.contains("_")){
                String[] parts = single_word.split("_");
                String part1 = parts[0];
                String part2 = parts[1];
                Sentence_type.add(new Tuple<String,String>(part1, part2));
            } 
        }
    }
    
    public boolean chech_negative(){
        Boolean negative = false;
        List<String> negative_list = Arrays.asList("zonder", "uitgezonderd", "behalve", "exclusief", "ex", "excl.", "excl");
        for (Tuple<String, String> element : Sentence_type) {
            String word_to_check = element.getX();
            if (negative_list.contains(word_to_check)) {
                negative = true;
            }
        }
        return negative;
    }
}    