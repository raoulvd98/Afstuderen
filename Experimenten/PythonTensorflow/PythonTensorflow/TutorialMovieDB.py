import pandas as pd
import glob
import os
import string
import tensorflow as tf
import pickle
# website: https://towardsdatascience.com/tensorflow-2-0-data-transformation-for-text-classification-b86ee2ad8877

def get_dfs(start_path):

  df = pd.DataFrame(columns=['text', 'sent'])
  text = []
  sent = []
  for p in ['pos','neg']:
    path=os.path.join(start_path, p)
    files = [f for f in os.listdir(path)
             if os.path.isfile(os.path.join(path,f))]
    for f in files:
      with open (os.path.join(path, f), "r", encoding='utf-8') as myfile:
        # replace carriage return linefeed with spaces
        text.append(myfile.read()
                    .replace("\n", " ")
                    .replace("\r", " "))
        # convert positive reviews to 1 and negative reviews to zero
        sent.append(1 if p == 'pos' else 0)

  df['text']=text
  df['sent']=sent
  #This line shuffles the data so you don't end up with contiguous
  #blocks of positive and negative reviews
  df = df.sample(frac=1).reset_index(drop=True)      
  return df

train_df = get_dfs ("C:/Users/raoul/.keras/datasets/aclImdb/train/")
test_df = get_dfs  ("C:/Users/raoul/.keras/datasets/aclImdb/test/")

#create tokenizer for our data
NUM_WORDS = 8000 # variabele of how many words we keep in our vocabulary
tokenizer = tf.keras.preprocessing.text.Tokenizer(num_words=NUM_WORDS, oov_token='<UNK>')
tokenizer.fit_on_texts(train_df['text'])

#convert text data to numerical indexes
train_seqs = tokenizer.texts_to_sequences(train_df['text'])
test_seqs = tokenizer.texts_to_sequences(test_df['text'])

#pad data up to SEQ_LEN (note that we truncate if there are more than SEQ_LEN tokens)
SEQ_LEN = 256 # variabele of how many words are used to check form each review
train_seqs = tf.keras.preprocessing.sequence.pad_sequences(train_seqs, maxlen=SEQ_LEN, padding="post")
test_seqs = tf.keras.preprocessing.sequence.pad_sequences(test_seqs, maxlen=SEQ_LEN, padding="post")

#
#   Create te model
#
EMBEDDING_SIZE = 64
model = tf.keras.Sequential([
    tf.keras.layers.Embedding(NUM_WORDS, EMBEDDING_SIZE),
    tf.keras.layers.GlobalAveragePooling1D(),
    tf.keras.layers.Dense(1, activation='sigmoid')])

model.summary()

model.compile(optimizer='adam',
              loss='binary_crossentropy',
              metrics=['accuracy'])


es = tf.keras.callbacks.EarlyStopping(monitor='val_accuracy', mode='max')
callbacks = [es]
BATCH_SIZE = 16 # variabele of how many examples are past to each batch of training
EPOCHS = 20 # number of training
history = model.fit(train_seqs, train_df['sent'].values
                    , batch_size=BATCH_SIZE
                    , epochs=EPOCHS
                    , validation_split=0.2
                    , callbacks=callbacks)

model.evaluate(test_seqs, test_df['sent'].values)

#
# SAVE THE MODEL
#

model.save('model.h5')
# saving
with open('tokenizer.pickle', 'wb') as handle:
    pickle.dump(tokenizer, handle, protocol=pickle.HIGHEST_PROTOCOL)

del model
del tokenizer

#
# USE THE MODEL
#

loaded_model=tf.keras.models.load_model('model.h5')

with open('tokenizer.pickle', 'rb') as f:
    loaded_tokenizer = pickle.load(f)

def prepare_predict_data(tokenizer, reviews):
  seqs = tokenizer.texts_to_sequences(reviews)
  seqs = tf.keras.preprocessing.sequence.pad_sequences(seqs, maxlen=SEQ_LEN, padding="post")
  return seqs

my_reviews = ['this movie was awesome',
              'this movie was the worst movie ive ever seen',
              'i hated everything about this movie',
              'this is my favorite movie of the year']
my_seqs = prepare_predict_data(loaded_tokenizer, my_reviews)

preds = loaded_model.predict(my_seqs)

pred_df = pd.DataFrame(columns=['text', 'sent'])
pred_df['text'] = my_reviews
pred_df['sent'] = preds

pred_df['sent'] = pred_df['sent'].apply(lambda x: 'pos' if x > 0.5 else 'neg')

print(pred_df)