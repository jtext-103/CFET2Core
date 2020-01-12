import msgpack
from io import BytesIO
import requests
import time

def GetContent(url):
    response = requests.get(url, headers={'Accept-Encoding':'MessagePack'})
    content = msgpack.unpackb(response.content, raw=False)
    return content

def GetVal(url):
    content = GetContent(url)
    val = content['CFET2CORE_SAMPLE_VAL']
    return val
    
def Invoke(url):
    requests.put(url, headers={'Accept-Encoding':'MessagePack'})

def Set(url):
    requests.post(url, headers={'Accept-Encoding':'MessagePack'})

Invoke('http://127.0.0.1:8002/fakecard/tryarm')
print(GetVal('http://127.0.0.1:8002/fakecard/aistate'))

Invoke('http://127.0.0.1:8002/fakecard/trystop')
print(GetVal('http://127.0.0.1:8002/fakecard/aistate'))

Set('http://127.0.0.1:8002/fakecard/channelCountConfig/10')
print(GetVal('http://127.0.0.1:8002/fakecard/channelCountConfig'))

Set('http://127.0.0.1:8002/fakecard/channelCountConfig/16')
print(GetVal('http://127.0.0.1:8002/fakecard/channelCount'))

print(GetVal('http://127.0.0.1:8002/fakecard/latestdata'))

