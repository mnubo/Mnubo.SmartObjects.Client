if [ -z "$VERSION" ]; then
    echo "\$VERSION variable is required"
    exit 1
fi  

if [ -z "$AUTHORS" ]; then
    AUTHORS=`git log --format='%aN' | grep -v "IEUser" | sort -u  | paste -sd, -`
fi  

if [ -z "$RELEASE_NOTES" ]; then
    echo "\$RELEASE_NOTES variable is required"
    exit 1
fi  

sed -e "s/\${VERSION}/$VERSION/g" -e "s/\${AUTHORS}/$AUTHORS/g" -e "s/\${RELEASE_NOTES}/$RELEASE_NOTES/g" Mnubo.SmartObjects.Client.nuspec.tmpl > Mnubo.SmartObjects.Client.nuspec

