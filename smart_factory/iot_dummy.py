import mysql.connector
import random
from datetime import datetime, timedelta

# MySQL 연결 설정
conn = mysql.connector.connect(
    host="localhost",
    database="smartfactory",
    user="root",
    password="mysql_p@ssw0rd",
)
cursor = conn.cursor()

# 파라미터
start_date = datetime(1997, 1, 1)  # 10,000일 전 기준
records_per_day = 100
total_days = 10000

insert_query = """
INSERT INTO iot_data (location, temperature, humidity, recorded_at)
VALUES (%s, %s, %s, %s)
"""

batch_size = 1000
batch = []

for day in range(total_days):
    date = start_date + timedelta(days=day)
    for i in range(records_per_day):
        timestamp = date + timedelta(minutes=15 * i)
        temperature = round(random.uniform(18.0, 28.0), 2)  # 18~28도
        humidity = round(random.uniform(30.0, 60.0), 2)     # 30~60%
        batch.append(('dinning', temperature, humidity, timestamp))

        if len(batch) >= batch_size:
            cursor.executemany(insert_query, batch)
            conn.commit()
            batch.clear()

# 남은 배치 처리
if batch:
    cursor.executemany(insert_query, batch)
    conn.commit()

cursor.close()
conn.close()
print("데이터 생성 완료!")
