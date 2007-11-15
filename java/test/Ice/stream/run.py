#!/usr/bin/env python
# **********************************************************************
#
# Copyright (c) 2003-2007 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

import os, sys

for toplevel in [".", "..", "../..", "../../..", "../../../..", "../../../../.."]:
    toplevel = os.path.normpath(toplevel)
    if os.path.exists(os.path.join(toplevel, "config", "TestUtil.py")):
        break
else:
    raise "can't find toplevel directory!"

sys.path.append(os.path.join(toplevel, "config"))
import TestUtil

name = os.path.join("Ice", "stream")
testdir = os.path.dirname(os.path.abspath(__file__))
os.environ["CLASSPATH"] = os.path.join(testdir, "classes") + os.pathsep + os.getenv("CLASSPATH", "")

print "starting test...",
clientPipe = TestUtil.startClient("Client"," 2>&1")
print "ok"

TestUtil.printOutputFromPipe(clientPipe)

clientStatus = TestUtil.closePipe(clientPipe)

if clientStatus:
    TestUtil.killServers()
    sys.exit(1)

sys.exit(0)
