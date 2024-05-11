import psycopg2
from psycopg2 import sql
import argparse
import time
import paho.mqtt.client as paho

subscription_topic = "sensor/pond-data"


def on_connect(client, userdata, flags, rc):
    print("Connected with result code " + str(rc))
    client.subscribe(subscription_topic + "/+")


def on_message(client, userdata, msg):
    print(msg.topic + " " + str(msg.payload))


def on_publish(client, userdata, mid):
    print("mid: " + str(mid))


def start_analytics():
    client = paho.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    client.on_publish = on_publish

    client.connect("localhost", 8883, 60)

    client.loop_forever()


if __name__ == "__main__":
    start_analytics()
