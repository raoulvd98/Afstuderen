import random
from pathlib import Path
import spacy
from spacy.util import minibatch, compounding

from Advertisements import *
from Rules_to_check import *
from Pipeline import *

x = Advertisements()

x.check_all_advertisement()