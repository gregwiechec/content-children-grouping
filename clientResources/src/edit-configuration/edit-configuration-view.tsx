import React, { useEffect, useState } from "react";
// @ts-ignore
import { useHistory, useParams } from "react-router-dom";
import { GeneratorConfiguration, GroupConfiguration } from "../models/group-configuration";
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
import { useServerSettingsContext } from "../server-settings";
import { useDataServiceContext } from "../data-service";
import { ContentLink } from "../content-link";
import { GeneratorsList } from "./generators-list";

interface EditConfigurationProps {
  onSaveSuccess: (message: string) => void;
}

export const EditConfigurationView = ({ onSaveSuccess }: EditConfigurationProps) => {
  const { editContentLink } = useParams();
  const dataService = useDataServiceContext();
  const {
    defaultContainerType,
    availableNameGenerators = [],
    options: { databaseConfigurationsEnabled = true, virtualContainersEnabled = true }
  } = useServerSettingsContext();

  const [isReadonly, setIsReadonly] = useState(!databaseConfigurationsEnabled);

  const history = useHistory();

  const [contentLink, setContentLink] = useState("");
  const [fromCode, setFromCode] = useState(false);
  const [containerTypeName, setContainerTypeName] = useState("");
  const [isRoutingEnabled, setIsRoutingEnabled] = useState(false);
  const [contentExists, setIsContentExists] = useState<boolean | undefined>(false);
  const [isVirtualContainer, setIsVirtualContainer] = useState(false);
  const [changedBy, setChangedBy] = useState<string | undefined>("");
  const [changedOn, setChangedOn] = useState<string | undefined>("");
  const [generators, setGenerators] = useState<GeneratorConfiguration[]>([]);

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
        setIsContentExists(result.contentExists);
        setChangedBy(result.changedBy);
        setChangedOn(result.changedOn);
      });
    } else {
      setGenerators([{ name: availableNameGenerators[0] }]);
    }
  }, [editContentLink, dataService, databaseConfigurationsEnabled, availableNameGenerators]);

  const onAddGenerator = () => {
    const updatedList = [...generators, { name: availableNameGenerators[0] }];
    setGenerators(updatedList);
  };

  const onGeneratorSettingsChanged = (index: number, settings: Record<string, string>) => {
    let updatedList = [...generators];
    updatedList[index].settings = settings;
    setGenerators(updatedList);
  };

  const onRemoveGenerator = (index: number) => {
    let updatedList = [...generators];
    updatedList.splice(index, 1);
    setGenerators(updatedList);
  };

  const onGeneratorValueChange = (index: number, value: string) => {
    let updatedList = [...generators];
    updatedList[index] = { name: value };
    setGenerators(updatedList);
  };

  const isValid = (): boolean => {
    if (!contentLink || contentLink.trim() === "") {
      return false;
    }

    return (generators || []).length !== 0;
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
    <GridContainer className="edit-configuration plugin-grid">
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
            <div className="text-description">Configuration registered from code cannot be edited</div>
          </GridCell>
        )}

        {!isEditing && (
          <GridCell large={12} medium={8} small={4}>
            <Input
                id="edit-configuration-content-link"
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
                <ContentLink value={editContentLink} contentExists={contentExists} />
              </span>
            </label>
          </GridCell>
        )}
        <GridCell large={12} medium={8} small={4}>
          <Checkbox
            label="Is virtual container"
            checked={isVirtualContainer}
            onChange={(e) => setIsVirtualContainer(e.target.checked)}
            isDisabled={isReadonly || !virtualContainersEnabled}
          />
          {!virtualContainersEnabled && <div className="text-description">Virtual containers are not enabled</div>}
        </GridCell>
          <GridCell>
              <Checkbox
                  label="Router enabled"
                  checked={isRoutingEnabled}
                  onChange={(e) => setIsRoutingEnabled(e.target.checked)}
                  isDisabled={isReadonly || isVirtualContainer}
              />
          </GridCell>
          <GridCell large={12} medium={8} small={4}>
              <Input
                  type="text"
                  id="edit-configuration-type"
                  label="Container type name"
                  note="Type format: [Full type name, Assembly Name]"
                  placeholder={defaultContainerType ? "Default: " + defaultContainerType : ""}
                  value={containerTypeName}
                  onChange={(e) => setContainerTypeName(e.target.value)}
                  isDisabled={isReadonly || isVirtualContainer}
              />
          </GridCell>
        <GridCell large={12} medium={8} small={4}>
          <Label>Name generators *</Label>
          <GeneratorsList
            generators={generators}
            isReadonly={isReadonly}
            onGeneratorValueChange={onGeneratorValueChange}
            onRemoveGenerator={onRemoveGenerator}
            onAddGenerator={onAddGenerator}
            onSettingsChanged={onGeneratorSettingsChanged}
            availableNameGenerators={availableNameGenerators}
            databaseConfigurationsEnabled={databaseConfigurationsEnabled}
          />
        </GridCell>

        {isEditing && !fromCode && (
          <GridCell large={12} medium={8} small={4}>
            <div>
              Changed by: <span style={{ fontWeight: "bold" }}>{changedBy}</span>
            </div>
            <div>
              Changed on: <span style={{ fontWeight: "bold" }}>{changedOn}</span>
            </div>
          </GridCell>
        )}

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
//TODO: when virtual container is true, then make router and container type readonly
