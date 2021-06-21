import pandas as pd
import glob
import os
import string
import tensorflow as tf
import pickle

def load_trainings_data(data_path):
    dataframe = pd.DataFrame(columns=['text', 'cat'])
    text = []
    cat = []
    for category in ['pos', 'neg']:
        path = os.path.join(data_path, category)
        files = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path, f))]
        for file in files:
            with open (os.path.join(path, file), "r", encoding='utf-8') as myfile:
                # replace carriage return linefeed with spaces
                text.append(myfile.read()
                            .replace("\n", " ")
                            .replace("\r", " "))
                # all good sentences turns to 1 and wrong sentences to 0
                cat.append(1 if category == 'pos' else 0)
    
    dataframe['text'] = text
    dataframe['cat'] = cat

    #This line shuffles the data so you don't end up with contiguous
    #blocks of samples from the same category
    dataframe = dataframe.sample(frac=1).reset_index(drop=True)      
    return dataframe

def convert_from_text_to_num(train_dataframe, test_dataframe, NUM_WORDS = 8000, SEQ_LEN = 256):
    #create tokenizer for our data
    tokenizer = tf.keras.preprocessing.text.Tokenizer(num_words=NUM_WORDS, oov_token='<UNK>')
    tokenizer.fit_on_texts(train_dataframe['text'])

    #convert text data to numerical indexes
    train_seqs = tokenizer.texts_to_sequences(train_dataframe['text'])
    test_seqs = tokenizer.texts_to_sequences(test_dataframe['text'])

    #pad data up to SEQ_LEN (note that we truncate if there are more than SEQ_LEN tokens)
    train_seqs = tf.keras.preprocessing.sequence.pad_sequences(train_seqs, maxlen=SEQ_LEN, padding="post")
    test_seqs = tf.keras.preprocessing.sequence.pad_sequences(test_seqs, maxlen=SEQ_LEN, padding="post")

    return tokenizer, train_seqs, test_seqs

def create_model(model_name = 'btw_model.h5', EMBEDDING_SIZE=64, NUM_WORDS = 8000, BATCH_SIZE = 16, EPOCHS = 3):
    #train_dataframe = load_trainings_data("C:/Users/raoul/git/repository/PythonTensorflow/Data/train/")
    #test_dataframe = load_trainings_data("C:/Users/raoul/git/repository/PythonTensorflow/Data/test/")
    train_dataframe = load_trainings_data("C:/Users/raoul/.keras/datasets/aclImdb/train/")
    test_dataframe = load_trainings_data("C:/Users/raoul/.keras/datasets/aclImdb/test/")

    tokenizer, train_seqs, test_seqs = convert_from_text_to_num(train_dataframe, test_dataframe)

    model = tf.keras.Sequential([
        tf.keras.layers.Embedding(NUM_WORDS, EMBEDDING_SIZE),
        tf.keras.layers.GlobalAveragePooling1D(),
        tf.keras.layers.Dense(1, activation='sigmoid')])

    model.summary()

    model.compile(optimizer='adam',
                  loss='binary_crossentropy',
                  metrics=['accuracy'])

    # Prevents overfitting
    es = tf.keras.callbacks.EarlyStopping(monitor='val_accuracy', mode='max')
    callbacks = [es]

    history = model.fit(train_seqs, train_dataframe['cat'].values
                    , batch_size=BATCH_SIZE
                    , epochs=EPOCHS
                    , validation_split=0.2
                    , callbacks=callbacks)

    model.evaluate(test_seqs, test_dataframe['cat'].values)
    
    # saving
    model.save(model_name)
   
    with open('tokenizer.pickle', 'wb') as handle:
        pickle.dump(tokenizer, handle, protocol=pickle.HIGHEST_PROTOCOL)

    del model
    del tokenizer

def load_existing_model(model_name = 'btw_model.h5'):
    loaded_model = tf.keras.models.load_model(model_name)

    with open('tokenizer.pickle', 'rb') as f:
        loaded_tokenizer = pickle.load(f)

    return loaded_model, loaded_tokenizer

def prepare_predict_data(tokenizer, data_to_check, SEQ_LEN = 256):
    cats = tokenizer.texts_to_sequences(data_to_check)
    cats = tf.keras.preprocessing.sequence.pad_sequences(cats, maxlen=SEQ_LEN, padding="post")
    return cats

def prediction(tokenizer, model, data, THRESHOLD = 0.5):
    new_cats = prepare_predict_data(tokenizer, data)
    preds = model.predict(new_cats)

    pred_dataframe = pd.DataFrame(columns=['text', 'cat'])
    pred_dataframe['text'] = data
    pred_dataframe['cat'] = preds

    pos = "Pos  "
    neg = "Neg  "
    pred_dataframe['cat'] = pred_dataframe['cat'].apply(
        lambda x: 
            pos + str(x) if x > THRESHOLD 
            else neg + str(x)
    )

    print(pred_dataframe)

def main():
    create_model()
    loaded_model, loaded_tokenizer = load_existing_model()

    #data_to_check = ['De prijs van deze auto is inlcusief btw',
    #                'De prijs van deze auto is exclusief btw',
    #                'Deze prijs in inclusief btw',
    #                'Deze prijs is exclusief btw']
    data_to_check = ['This movie is really awesome',
                    'this is a very bad movie',
                    'I am in love with this movie',
                    'there is no worse film',
                    'the movie was ok',
                    'this was a terrible movie',
                    'There are better films',]

    prediction(loaded_tokenizer, loaded_model, data_to_check)

main()