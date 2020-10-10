import QtQuick 2.9
import QtQuick.Window 2.2

Rectangle {
    visible: true
    width: 3840
    height: 2160

    color : "#00000000"


    Rectangle
    {
        anchors.centerIn: parent
        color: "red"
        width: 500
        height: 500
        radius: 250
    }
}
