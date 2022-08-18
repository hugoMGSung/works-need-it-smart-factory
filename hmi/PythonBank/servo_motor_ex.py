import RPi.GPIO as GPIO
import time

import paho.mqtt.client as mqtt
import json
import datetime as dt
import RPi.GPIO as GPI

servoPin = 17   # 서보핀
SERVO_MAX = 12  # 서보모터의 최대(180도) 위치 주기
SERVO_MIN = 3   # 서보모터의 최소(0도) 위치 주기

GPIO.setmode(GPIO.BCM)
GPIO.setup(servoPin, GPIO.OUT)

servo = GPIO.PWM(servoPin, 50)
servo.start(0)  

def on_connect(client, userdata, flags, rc): 
    print("Connected with result code {0}".format(str(rc)))
    client.subscribe("machine01/4002/") 

def on_message(client, userdata, msg):
    print("Message received-> " + msg.topic + " " + str(msg.payload)) 
    print(msg.payload.decode('utf-8'))
    data = json.loads(msg.payload.decode('utf-8'))
    print(data["motor"])
    setServoPos(int(data["motor"]))
    time.sleep(1)
    setServoPos(0)

def setServoPos(degree):
    if degree > 180:
        degree = 180

    # degree를 duty로 변경
    duty = SERVO_MIN + (degree*(SERVO_MAX-SERVO_MIN)/180.0)
    # duty value 출력
    print('Degree: {} to {}(Duty)'.format(degree, duty))

    # 변경된 duty value를 서보 PWM 적용
    servo.ChangeDutyCycle(duty)

try:
    client = mqtt.Client("servo_motor")  # Create instance of client with client ID “digi_mqtt_test”
    client.on_connect = on_connect  # Define callback function for successful connection
    client.on_message = on_message  # Define callback function for receipt of a message
    # client.connect("m2m.eclipse.org", 1883, 60)  # Connect to (broker, port, keepalive-time)
    client.connect('192.168.200.102', 1883)
    client.loop_forever()
except KeyboardInterrupt:
    servo.stop()
    GPIO.cleanup()