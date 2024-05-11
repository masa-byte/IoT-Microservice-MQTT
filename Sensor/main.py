import psycopg2
import argparse
import time
import paho.mqtt.publish as publish
import threading

topic = "sensor/pond-data"


def get_pond_ids():
    try:
        connection = psycopg2.connect(
            database="postgres",
            user="postgres",
            password="postgres",
            host="localhost",
            port="5432",
        )
        cursor = connection.cursor()

        sql = """SELECT DISTINCT("PondId") FROM "PondData" """
        cursor.execute(sql)
        records = cursor.fetchall()

        cursor.close()
        connection.close()

        pond_ids = []
        for row in records:
            pond_ids.append(row[0])

        return pond_ids

    except (Exception, psycopg2.Error) as error:
        print("Error while fetching data from PostgreSQL", error)
        return 0


def load_data_from_db(pond_id, limit, offset):
    try:
        connection = psycopg2.connect(
            database="postgres",
            user="postgres",
            password="postgres",
            host="localhost",
            port="5432",
        )
        cursor = connection.cursor()

        sql = """
            SELECT * FROM "PondData"
            WHERE "PondId" = %s
            ORDER BY "EntryId" ASC LIMIT %s OFFSET %s
        """

        cursor.execute(sql, (pond_id, limit, offset))
        records = cursor.fetchall()

        for row in records:
            print(threading.current_thread().name, row)
            try:
                publish.single(
                    topic + "/" + str(pond_id),
                    payload=str(row),
                    hostname="localhost",
                    port=8883,
                    client_id="",
                    keepalive=60,
                    qos=0,
                    retain=False,
                )
            except Exception as e:
                print("Error while publishing message", e)

        cursor.close()
        connection.close()

    except (Exception, psycopg2.Error) as error:
        print("Error while fetching data from PostgreSQL", error)


def process_pond_data(pond_id, limit, sleep):
    offset = 0

    while True:
        load_data_from_db(pond_id, limit, offset)
        offset += limit
        time.sleep(sleep)


def start_sensors(pond_ids, limit, sleep):
    threads = []

    for pond_id in pond_ids:
        thread = threading.Thread(
            target=process_pond_data, args=(pond_id, limit, sleep)
        )
        threads.append(thread)
        thread.start()

    for thread in threads:
        thread.join()


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--limit", type=int, default=10, help="Limit the number of records to fetch"
    )
    parser.add_argument(
        "--sleep", type=int, default=10, help="Sleep time in seconds between each fetch"
    )

    args = parser.parse_args()

    limit = args.limit
    sleep = args.sleep

    pond_ids = get_pond_ids()

    start_sensors(pond_ids, limit, sleep)
