#!/usr/bin/env bash

git submodule update --init --recursive

git apply disable-windows-editor.patch
