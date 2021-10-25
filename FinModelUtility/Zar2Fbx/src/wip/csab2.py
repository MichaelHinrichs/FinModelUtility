# Shamelessly based on https://github.com/magcius/noclip.website/blob/e7da91f0d8fcef6ea58659e991fd6408b940194e/src/oot3d/csab.ts

import math
from .io_utils import (readDataType, readString, readArray, readFloat, readSn16, readUn16,
                    readInt16, readUInt16, readUInt32, readInt32, readUShort,
                    readShort, readByte, readUByte)
from .common import GLOBAL_SCALE

ANIMATION_TRACK_TYPE_LINEAR = 0x01
ANIMATION_TRACK_TYPE_HERMITE = 0x02
ANIMATION_TRACK_TYPE_INTEGER = 0x03


class AnimationKeyframeHermite:
    def __init__(self):
        self.time = -1
        self.value = -1
        self.tangentIn = -1
        self.tangentOut = -1

class AnimationTrackLinear:
    def __init__(self):
        self.type = ANIMATION_TRACK_TYPE_LINEAR
        self.frames = []

class AnimationTrackHermite:
    def __init__(self):
        self.type = ANIMATION_TRACK_TYPE_HERMITE
        self.timeEnd = -1
        self.frames = []

    def __init__(self):
        self.type = ANIMATION_TRACK_TYPE_INTEGER
        self.frames = []


LOOP_MODE_ONCE = 0
LOOP_MODE_REPEAT = 1

class AnimationNode:
    def __init__(self):
        self.boneIndex = -1
        self.scaleX = None
        self.rotationX = None
        self.translationX = None
        self.scaleY = None
        self.rotationY = None
        self.translationY = None
        self.scaleZ = None
        self.rotationZ = None
        self.translationZ = None

class CSAB:
    def __init__(self):
        self.duration = -1
        self.loopMode = 0
        self.animationNodes = []
        self.boneToAnimationTable = []

def align(n, multiple):
    mask = multiple - 1
    return (n + mask) & ~mask



