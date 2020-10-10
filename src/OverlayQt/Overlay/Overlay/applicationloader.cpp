#include "applicationloader.h"

ApplicationLoader::ApplicationLoader(QObject* parent) : QObject(parent)
{

}

void ApplicationLoader::run(){

    m_engine.setSource(QStringLiteral("qrc:/main.qml"));
    m_engine.setFlag(Qt::FramelessWindowHint);
    m_engine.setColor(Qt::transparent);
    m_engine.show();
}
