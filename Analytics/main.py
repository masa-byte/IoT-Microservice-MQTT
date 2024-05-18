import paho.mqtt.client as paho
import json
import datetime

sensor_topic = "sensor/pond-data"  # this topic is separated for each pond
ekuiper_send_topic = "ekuiper/ponds-data"  # this topic is grouped for all ponds
ekuiper_receive_topic = (
    "ekuiper/ponds-data-result"  # this topic is grouped for all ponds
)
analytics_topic = "analytics/pond-data"  # this topic is separated for each pond


def on_connect(client, userdata, flags, rc):
    print("Connected with result code " + str(rc))
    client.subscribe(sensor_topic + "/+")
    client.subscribe(ekuiper_receive_topic)


def on_message(client, userdata, msg):

    if msg.topic.startswith(sensor_topic):
        dict_data = json.loads(msg.payload)
        # print("Message received from sensor topic:", dict_data)
        json_data = json.dumps(dict_data)
        client.publish(ekuiper_send_topic, payload=json_data, qos=1)

    elif msg.topic == ekuiper_receive_topic:
        print("Message received from ekuiper receive topic:", msg.payload)
        if msg.payload:
            dict_data = json.loads(msg.payload)
            dict_data["Timestamp"] = datetime.datetime.now().strftime(
                "%Y-%m-%d %H:%M:%S"
            )

            if "AvgTemperature_C" in dict_data:
                if dict_data["AvgTemperature_C"] > 24:
                    dict_data["EventType"] = "HighTemperatureAlarm"
                elif dict_data["AvgTemperature_C"] < 18:
                    dict_data["EventType"] = "LowTemperatureAlarm"

            if "MaxpH" in dict_data:
                if dict_data["MaxpH"] > 9:
                    dict_data["EventType"] = "HighPHAlarm"
                elif dict_data["MaxpH"] < 5:
                    dict_data["EventType"] = "LowPHAlarm"

            json_data = json.dumps(dict_data)
            print(json_data)
            pond_id = dict_data["PondId"]
            client.publish(
                analytics_topic + "/" + str(pond_id), payload=json_data, qos=1
            )

    else:
        print("Message received from an unknown topic:", msg.topic)


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
