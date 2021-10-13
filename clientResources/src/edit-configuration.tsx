import React, { useEffect, useState } from "react";
// @ts-ignore
import { useHistory, useParams } from "react-router-dom";
import { GroupConfiguration } from "./models/Groupconfiguration";
import {
  Attention,
  BlockList,
  Button,
  ButtonIcon,
  Checkbox,
  Grid,
  GridCell,
  GridContainer,
  Input,
  Label,
  Select
} from "optimizely-oui";
import { useServerSettingsContext } from "./server-settings";
import { useDataServiceContext } from "./data-service";
import { ContentLink } from "./ContentLink";

interface EditConfigurationProps {
  onSaveSuccess: (message: string) => void;
}

export const EditConfiguration = ({ onSaveSuccess }: EditConfigurationProps) => {
  const { editContentLink } = useParams();
  const dataService = useDataServiceContext();
  const {
    availableNameGenerators = [],
    options: { databaseConfigurationsEnabled = true }
  } = useServerSettingsContext();

  const [isReadonly, setIsReadonly] = useState(!databaseConfigurationsEnabled);

  const history = useHistory();

  const [contentLink, setContentLink] = useState("");
  const [fromCode, setFromCode] = useState(false);
  const [containerTypeName, setContainerTypeName] = useState("");
  const [isRoutingEnabled, setIsRoutingEnabled] = useState(false);
  const [isVirtualContainer, setIsVirtualContainer] = useState(false);
  const [generators, setGenerators] = useState<string[]>([]);

  const [validationMessage, setValidationMessage] = useState("");

  useEffect(() => {
    if (editContentLink) {
      dataService?.get(editContentLink).then((result: GroupConfiguration) => {
        if (!result) {
          return;
        }
        setContentLink(result.contentLink);
        setFromCode(result.fromCode);
        setContainerTypeName(result.containerTypeName);
        setIsRoutingEnabled(result.routingEnabled);
        setIsVirtualContainer(result.isVirtualContainer);
        setGenerators(result.groupLevelConfigurations || []);
        setIsReadonly(!databaseConfigurationsEnabled || result.fromCode);
      });
    } else {
      setGenerators([availableNameGenerators[0]]);
    }
  }, [editContentLink, dataService]);

  const onAddGenerator = () => {
    const updatedList = [...generators, availableNameGenerators[0]];
    setGenerators(updatedList);
  };

  const onRemoveGenerator = (index: number) => {
    let updatedList = [...generators];
    updatedList.splice(index, 1);
    setGenerators(updatedList);
  };

  const onGeneratorValueChange = (index: number, value: string) => {
    let updatedList = [...generators];
    updatedList[index] = value;
    setGenerators(updatedList);
  };

  const isValid = (): boolean => {
    if (!contentLink || contentLink.trim() === "") {
      return false;
    }

    if (!generators || generators.length === 0) {
      return false;
    }

    return true;
  };

  const onSave = () => {
    dataService
      .save({
        isNew: !isEditing,
        contentLink: contentLink || editContentLink,
        fromCode: false,
        containerTypeName: containerTypeName,
        routingEnabled: isRoutingEnabled,
        isVirtualContainer: isVirtualContainer,
        groupLevelConfigurations: generators
      })
      .then(() => {
        onSaveSuccess("Configuration saved");
        history.push("/");
      })
      .catch((error) => {
        setValidationMessage(error);
      });
  };

  const isEditing = !!editContentLink;

  return (
    <GridContainer className="edit-configuration">
      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <h2>{isEditing ? "Edit configuration" : "Add configuration"}</h2>
        </GridCell>

        {validationMessage && (
          <GridCell large={12} medium={8} small={4}>
            <Attention alignment="center" type="bad-news">
              {validationMessage}
            </Attention>
          </GridCell>
        )}

        {fromCode && (
          <GridCell large={12} medium={8} small={4}>
            <div style={{ fontStyle: "italic" }}>Configuration registered from code cannot be edited</div>
          </GridCell>
        )}

        {!isEditing && (
          <GridCell large={12} medium={8} small={4}>
            <Input
              displayError={false}
              type="number"
              isOptional={false}
              label="Container Content link"
              maxLength={5}
              className="content-link"
              min={1}
              onChange={(e) => setContentLink(e.target.value)}
              value={contentLink}
              isRequired
              isDisabled={isReadonly}
            />
          </GridCell>
        )}

        {isEditing && (
          <GridCell large={12} medium={8} small={4}>
            <label>
              {fromCode && <>Content link&nbsp;</>}
              {!fromCode && <>Editing content link&nbsp;</>}
              <span style={{ fontWeight: "bold" }}>
                <ContentLink value={editContentLink} />
              </span>
            </label>
          </GridCell>
        )}
        <GridCell large={12} medium={8} small={4}>
          <Checkbox
            label="Is virtual container"
            checked={isVirtualContainer}
            onChange={(e) => setIsVirtualContainer(e.target.checked)}
            isDisabled={isReadonly}
          />
        </GridCell>

        {!isVirtualContainer && (
          <>
            <GridCell>
              <Checkbox
                label="Router enabled"
                checked={isRoutingEnabled}
                onChange={(e) => setIsRoutingEnabled(e.target.checked)}
                isDisabled={isReadonly}
              />
            </GridCell>
            <GridCell large={12} medium={8} small={4}>
              <Input
                type="text"
                label="Container type name"
                note="Type format: [Full type name, Assembly Name]"
                value={containerTypeName}
                onChange={(e) => setContainerTypeName(e.target.value)}
                isDisabled={isReadonly}
              />
            </GridCell>
          </>
        )}
        <GridCell large={12} medium={8} small={4}>
          <Label>Name generators *</Label>
          <BlockList hasBorder={false} className="configuration-item">
            {generators.map((x, index) => (
              <BlockList.Item key={x + "_" + index}>
                {!isReadonly && (
                  <>
                    <Select
                      className="configuration-generator-select"
                      isOptional={false}
                      onChange={(value) => onGeneratorValueChange(index, value.target.value)}
                    >
                      {/*TODO: allow to configure namegenerator*/}
                      {availableNameGenerators.map((generator) => (
                        <option key={generator} value={generator} selected={generator === x}>
                          {generator}
                        </option>
                      ))}
                    </Select>
                    <ButtonIcon
                      className="remove-button"
                      iconName="close"
                      isDisabled={index === 0}
                      onClick={() => onRemoveGenerator(index)}
                      size="small"
                      style="outline"
                      title="Close Dialog"
                    />
                  </>
                )}

                {isReadonly && <span key={x}>{x}</span>}
              </BlockList.Item>
            ))}
          </BlockList>
          {!isReadonly && (
            <Button
              style="outline"
              size="narrow"
              leftIcon="add"
              onClick={onAddGenerator}
              isDisabled={databaseConfigurationsEnabled && (availableNameGenerators?.length || 0) === 0}
            >
              Add generator
            </Button>
          )}
        </GridCell>

        {!isReadonly && (
          <GridCell large={12} medium={8} small={4}>
            <Button style="plain" key={0} onClick={() => history.push("/")}>
              Cancel
            </Button>
            &nbsp;
            <Button isDisabled={!isValid()} style="highlight" key={1} onClick={onSave}>
              Save
            </Button>
          </GridCell>
        )}
        {isReadonly && (
          <GridCell large={12} medium={8} small={4}>
            <Button style="plain" key={0} onClick={() => history.push("/")}>
              Back
            </Button>
          </GridCell>
        )}
      </Grid>
    </GridContainer>
  );
};
