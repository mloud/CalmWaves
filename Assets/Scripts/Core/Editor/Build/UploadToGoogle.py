import argparse
import json
import os
import datetime
import sys

from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload
from google.oauth2.service_account import Credentials

def log(message):
    print(f"{datetime.datetime.now()}: {message}")

def main():
    parser = argparse.ArgumentParser(description="Upload Build Script with JSON")
    parser.add_argument("--service_account_json_file", required=True, help="Service account JSON file")
    parser.add_argument("--bundle_id", required=True, help="Application bundle ID")
    parser.add_argument("--version_code", required=True, help="Version code of the build")
    parser.add_argument("--aab_file", required=True, help="AAB file")
    parser.add_argument("--track", default="internal", help="Track to upload the build (default: internal)")
    
    args = parser.parse_args()

    log(f"Service Account JSON: {args.service_account_json_file}")
    log(f"Bundle ID: {args.bundle_id}")
    log(f"Version Code: {args.version_code}")
    log(f"AAB file: {args.aab_file}")
    
    if not os.path.exists(args.service_account_json_file):
        log(f"Error: Service account file not found at {args.service_account_json_file}")
        sys.exit(1)
    if not os.path.exists(args.aab_file):
        log(f"Error: AAB file not found at {args.aab_file}")
        sys.exit(1)
    
    try:
        credentials = Credentials.from_service_account_file(
            args.service_account_json_file,
            scopes=["https://www.googleapis.com/auth/androidpublisher"]
        )
        service = build("androidpublisher", "v3", credentials=credentials)
    except Exception as e:
        log(f"Error authenticating with the Google Play API: {e}")
        sys.exit(1)

    try:
        edit_request = service.edits().insert(body={}, packageName=args.bundle_id)
        edit = edit_request.execute()

        aab_request = service.edits().bundles().upload(
            editId=edit["id"],
            packageName=args.bundle_id,
            media_body=MediaFileUpload(args.aab_file, mimetype="application/octet-stream")
        )
        aab_response = aab_request.execute()
        log(f"Uploaded AAB version: {aab_response['versionCode']}")

        track_request = service.edits().tracks().update(
            editId=edit["id"],
            packageName=args.bundle_id,
            track=args.track,
            body={
                "releases": [
                    {
                        "name": "Internal Test Release",
                        "status": "completed",
                        "versionCodes": [aab_response["versionCode"]]
                    }
                ]
            }
        )
        track_response = track_request.execute()
        log(f"Track updated: {track_response['track']}")

        commit_request = service.edits().commit(editId=edit["id"], packageName=args.bundle_id)
        commit_response = commit_request.execute()
        log("Changes committed successfully!")
    except Exception as e:
        log(f"An error occurred during upload: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()