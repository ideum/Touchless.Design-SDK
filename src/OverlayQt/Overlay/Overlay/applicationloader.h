#ifndef APPLICATIONLOADER_H
#define APPLICATIONLOADER_H

#include <QObject>
#include <QQuickView>
#include <QDebug>

class ApplicationLoader : public QObject
{
    Q_OBJECT
public:
    ApplicationLoader(QObject *parent = 0);

    void run();

private:

    QQuickView m_engine;
};

#endif // APPLICATIONLOADER_H
