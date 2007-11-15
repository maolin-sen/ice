# **********************************************************************
#
# Copyright (c) 2003-2007 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

#
# Set value to 1 if you are building Ice for Ruby against an RPM installation
# of Ice.
#
USE_ICE_RPM ?= 0

USE_SRC_DIST = 0

#
# Checks for ICE_HOME environment variable.  If it isn't present,
# attempt to find an Ice installation in /usr or the default install
# location.
#
ifneq ($(ICE_HOME),)
    ifeq ($(USE_ICE_RPM), 1)
$(error Ambiguous directives. Both ICE_HOME and USE_ICE_RPM are defined.)
    endif

    ICE_DIR = $(ICE_HOME)

    ifneq ($(shell test -f $(ICE_DIR)/bin/slice2cs && echo 0),0)
$(error Unable to locate slice2cs in $(ICE_DIR). Please verify ICE_HOME is properly configured and Ice is correctly installed.)
    endif
else
    ifeq ($(USE_ICE_RPM),1)
        ICE_DIR=/usr
    
	ifneq ($(shell test -f $(ICE_DIR)/bin/slice2cs && echo 0),0)
$(error Unable to locate slice2cs in $(ICE_DIR). Please verify that the required RPMS are properly installed.)
	endif
    else
        ICE_DIR = $(top_srcdir)/..
        USE_SRC_DIST = 1
    endif
endif

#
# If you are compiling with MONO you must define this symbol.
#
MONO = yes

#
# Select an installation base directory. The directory will be created
# if it does not exist.
#

prefix			?= /opt/IceCS-$(VERSION)

#
# The default behavior of 'make install' attempts to add the Ice for C#
# libraries to the Global Assembly Cache (GAC). If you would prefer not
# to install these libraries to the GAC, or if you do not have sufficient
# privileges to do so, then enable no_gac and the libraries will be
# copied to $(prefix)/bin instead.
#

#no_gac			= 1

#
# Define DEBUG as yes if you want to build with debug information and
# assertions enabled.
#

DEBUG			= yes

#OPTIMIZE		= yes

# ----------------------------------------------------------------------
# Don't change anything below this line!
# ----------------------------------------------------------------------


ifeq ($(MONO), yes)
	DSEP = /
else
	DSEP = \\
endif

SHELL			= /bin/sh
VERSION			= 3.3.0

ifneq ($(ICE_DIR),)
    ifneq ($(USE_SRC_DIST),0)
        ifeq ($(LP64),yes)
            export LD_LIBRARY_PATH := $(ICE_DIR)/cpp/lib64:$(LD_LIBRARY_PATH)
        else
            export LD_LIBRARY_PATH := $(ICE_DIR)/cpp/lib:$(LD_LIBRARY_PATH)
        endif
    else
        ifeq ($(LP64),yes)
            export LD_LIBRARY_PATH := $(ICE_DIR)/lib64:$(LD_LIBRARY_PATH)
        else
            export LD_LIBRARY_PATH := $(ICE_DIR)/lib:$(LD_LIBRARY_PATH)
        endif
    endif
endif

bindir			= $(top_srcdir)/bin
libdir			= $(top_srcdir)/lib

#
# If a slice directory is contained along with this distribution -- use it. 
# Otherwise use paths relative to $(ICE_DIR).
#
ifneq ($(USE_ICE_RPM),0)
    slicedir		= /usr/share/Ice-$(VERSION)/slice
else
    ifeq ($(shell test -d $(ICE_DIR)/slice && echo 0),0)
        slicedir		= $(ICE_DIR)/slice
    else
        slicedir		= $(ICE_DIR)/../slice
    endif
endif



install_bindir		= $(prefix)/bin
install_libdir		= $(prefix)/lib
install_slicedir	= $(prefix)/slice

ifneq ($(ICE_DIR),/usr)
ref = -r:$(bindir)/$(1).dll
else
ref = -pkg:$(1)
endif

ifdef no_gac
NOGAC			?= $(no_gac)
endif

INSTALL			= cp -fp
INSTALL_PROGRAM		= ${INSTALL}
INSTALL_LIBRARY		= ${INSTALL}
INSTALL_DATA		= ${INSTALL}

GACUTIL			= gacutil

ifeq ($(MONO),yes)
MCS			= gmcs
else
MCS			= csc -nologo
endif

LIBS			= $(bindir)/icecs.dll $(bindir)/glaciercs.dll

MCSFLAGS = -warnaserror -d:MAKEFILE_BUILD
ifeq ($(DEBUG),yes)
    MCSFLAGS := $(MCSFLAGS) -debug -define:DEBUG
endif

ifeq ($(OPTIMIZE),yes)
    MCSFLAGS := $(MCSFLAGS) -optimize+
endif

ifeq ($(installdata),)
    installdata		= $(INSTALL_DATA) $(1) $(2); \
			  chmod a+r $(2)/$(notdir $(1))
endif

ifeq ($(installprogram),)
    installprogram	= $(INSTALL_PROGRAM) $(1) $(2); \
			  chmod a+rx $(2)/$(notdir $(1))
endif

ifeq ($(installlibrary),)
    installlibrary	= $(INSTALL_LIBRARY) $(1) $(2); \
			  chmod a+rx $(2)/$(notdir $(1))
endif

ifeq ($(mkdir),)
    mkdir		= mkdir $(1) ; \
			  chmod a+rx $(1)
endif

SLICE2CS		= $(ICE_DIR)/bin/slice2cs

GEN_SRCS = $(subst .ice,.cs,$(addprefix $(GDIR)/,$(notdir $(SLICE_SRCS))))
CGEN_SRCS = $(subst .ice,.cs,$(addprefix $(GDIR)/,$(notdir $(SLICE_C_SRCS))))
SGEN_SRCS = $(subst .ice,.cs,$(addprefix $(GDIR)/,$(notdir $(SLICE_S_SRCS))))
GEN_AMD_SRCS = $(subst .ice,.cs,$(addprefix $(GDIR)/,$(notdir $(SLICE_AMD_SRCS))))
SAMD_GEN_SRCS = $(subst .ice,.cs,$(addprefix $(GDIR)/,$(notdir $(SLICE_SAMD_SRCS))))


EVERYTHING		= all depend clean install config

.SUFFIXES:
.SUFFIXES:		.cs .ice

%.cs: %.ice
	$(SLICE2CS) $(SLICE2CSFLAGS) $<

$(GDIR)/%.cs: $(SDIR)/%.ice
	$(SLICE2CS) --output-dir $(GDIR) $(SLICE2CSFLAGS) $<

all:: $(TARGETS)

depend:: $(SLICE_SRCS) $(SLICE_C_SRCS) $(SLICE_S_SRCS) $(SLICE_AMD_SRCS) $(SLICE_SAMD_SRCS)
	-rm -f .depend
	if test -n "$(SLICE_SRCS)" ; then \
	    $(SLICE2CS) --depend $(SLICE2CSFLAGS) $(SLICE_SRCS) | \
	    $(top_srcdir)/config/makedepend.py >> .depend; \
	fi
	if test -n "$(SLICE_C_SRCS)" ; then \
	    $(SLICE2CS) --depend $(SLICE2CSFLAGS) $(SLICE_C_SRCS) | \
	    $(top_srcdir)/config/makedepend.py >> .depend; \
	fi
	if test -n "$(SLICE_S_SRCS)" ; then \
	    $(SLICE2CS) --depend $(SLICE2CSFLAGS) $(SLICE_S_SRCS) | \
	    $(top_srcdir)/config/makedepend.py >> .depend; \
	fi
	if test -n "$(SLICE_AMD_SRCS)" ; then \
	    $(SLICE2CS) --depend $(SLICE2CSFLAGS) $(SLICE_AMD_SRCS) | \
	    $(top_srcdir)/config/makedepend.py >> .depend; \
	fi
	if test -n "$(SLICE_SAMD_SRCS)" ; then \
	    $(SLICE2CS) --depend $(SLICE2CSFLAGS) $(SLICE_SAMD_SRCS) | \
	    $(top_srcdir)/config/makedepend.py >> .depend; \
	fi

clean::
	-rm -f $(TARGETS) $(patsubst %,%.mdb,$(TARGETS)) *.bak *.dll *.pdb *.mdb

config::
	$(top_srcdir)/config/makeconfig.py $(top_srcdir) $(TARGETS)

ifneq ($(SLICE_SRCS),)
clean::
	-rm -f $(GEN_SRCS)
endif
ifneq ($(SLICE_C_SRCS),)
clean::
	-rm -f $(CGEN_SRCS)
endif
ifneq ($(SLICE_S_SRCS),)
clean::
	-rm -f $(SGEN_SRCS)
endif
ifneq ($(SLICE_AMD_SRCS),)
clean::
	-rm -f $(GEN_AMD_SRCS)
endif
ifneq ($(SLICE_SAMD_SRCS),)
clean::
	-rm -f $(SAMD_GEN_SRCS)
endif

install::
