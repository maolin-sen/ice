// **********************************************************************
//
// Copyright (c) 2001
// MutableRealms, Inc.
// Huntsville, AL, USA
//
// All Rights Reserved
//
// **********************************************************************

// TODO: ML: Please reformat to comply with our coding conventions.

struct s1 {};			// Illegal empty struct
struct s2 { long l; };		// One member, OK
struct s3 { [ ] long l; };	// One member, OK
struct s4 { [ "Hi" ] long l; };	// One member with metadata, OK
struct s5 {			// Two members, OK
	long l;
	byte b;
};
struct s6 {			// Two members with metadata, OK
	[ "Hi" ]	long l;
			byte b;
};
struct s7 {			// Two members with metadata, OK
	[ "Hi" ]	long l;
	[ ] 		byte b;
};

struct s8 { [ ] long ; };	// Missing data member name
struct s9 { [ ] long };		// Missing data member name
struct s10 { long ; };		// Missing data member name
struct s11 { long };		// Missing data member name
