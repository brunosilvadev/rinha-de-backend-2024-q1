#!/bin/bash
set -e

# Adjust max_connections
echo "max_connections = 250" >> /var/lib/postgresql/data/postgresql.conf