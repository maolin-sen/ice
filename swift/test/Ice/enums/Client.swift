//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

import Ice
import TestCommon

public class Client: TestHelperI {
    public override func run(args _: [String]) throws {
        let writer = getWriter()
        writer.write("testing default values... ")
        writer.writeLine("ok")
    }
}
