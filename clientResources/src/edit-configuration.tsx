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

interface EditConfigurationProps {
  onSaveSuccess: (message: string) => void;
}

export const EditConfiguration = ({ onSaveSuccess }: EditConfigurationProps) => {
  const { editContentLink } = useParams();
  const dataService = useDataServiceContext();
  const { availableNameGenerators = [] } = useServerSettingsContext();

  const history = useHistory();
  const [configuration, setConfiguration] = useState<GroupConfiguration | null>(null);

  const [contentLink, setContentLink] = useState("");
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
        setConfiguration(result);
        setContentLink(result.contentLink);
        setContainerTypeName(result.containerTypeName);
        setIsRoutingEnabled(result.routingEnabled);
        setIsVirtualContainer(result.isVirtualContainer);
        setGenerators(result.groupLevelConfigurations || []);
      });
    }
  }, [editContentLink, dataService]);

  useEffect(() => {
    setContentLink(configuration?.contentLink || "");
    setContainerTypeName(configuration?.containerTypeName || "");
    setIsRoutingEnabled(configuration?.routingEnabled || false);
    setGenerators(configuration?.groupLevelConfigurations || []);
  }, [configuration]);

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
    dataService.save({
      isNew: !isEditing,
      contentLink: contentLink || editContentLink,
      fromCode: false,
      containerTypeName: containerTypeName,
      routingEnabled: isRoutingEnabled,
      isVirtualContainer: isVirtualContainer,
      groupLevelConfigurations: generators
    }).then(() => {
      onSaveSuccess("Configuration saved");
      history.push("/");
    }).catch(error => {
      setValidationMessage(error.message);
    })
  };

  const isEditing = !!editContentLink;

  return (
    <GridContainer className="configuration-item">
      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <h2>{isEditing ? "Edit configuration" : "Add configuration"}</h2>
        </GridCell>

        {validationMessage && (
          <Attention alignment="center" type="bad-news">
            {validationMessage}
          </Attention>
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
            />
          </GridCell>
        )}

        {isEditing && (
          <GridCell large={12} medium={8} small={4}>
            <label>Content link {editContentLink}</label>
          </GridCell>
        )}
        <GridCell large={12} medium={8} small={4}>
          <Checkbox
            label="Is virtual container"
            checked={isVirtualContainer}
            onChange={(e) => setIsVirtualContainer(e.target.checked)}
          />
        </GridCell>

        {!isVirtualContainer && (
          <>
            <GridCell large={12} medium={8} small={4}>
              <Input
                type="text"
                label="Container type name"
                note="Type format: [Full type name, Assembly Name]"
                value={containerTypeName}
                onChange={(e) => setContainerTypeName(e.target.value)}
              />
            </GridCell>
            <GridCell>
              <Checkbox
                label="Router enabled"
                checked={isRoutingEnabled}
                onChange={(e) => setIsRoutingEnabled(e.target.checked)}
              />
            </GridCell>
          </>
        )}
        <GridCell large={12} medium={8} small={4}>
          <Label>Name generators *</Label>
          <BlockList hasBorder={false} className="configuration-item">
            {generators.map((x, index) => (
              <BlockList.Item key={x + "_" + index}>
                <Select
                  className="configuration-generator-select"
                  isOptional={false}
                  onChange={(value) => onGeneratorValueChange(index, value.target.value)}
                >
                  {/*allow to configure namegenerator*/}
                  {availableNameGenerators.map((generator) => (
                    <option key={generator} value={generator} selected={generator === x}>
                      {generator}
                    </option>
                  ))}
                </Select>
                {index > 0 && (
                  <ButtonIcon
                    className="remove-button"
                    iconName="close"
                    isDisabled={false}
                    onClick={() => onRemoveGenerator(index)}
                    size="small"
                    style="outline"
                    title="Close Dialog"
                  />
                )}
              </BlockList.Item>
            ))}
          </BlockList>
          <Button
            style="outline"
            size="narrow"
            leftIcon="add"
            onClick={onAddGenerator}
            isDisabled={(availableNameGenerators?.length || 0) === 0}
          >
            Add generator
          </Button>
        </GridCell>
      </Grid>

      <Grid>
        <GridCell large={12} medium={8} small={4}>
          <br />
          <Button style="plain" key={0} onClick={() => history.push("/")}>
            Cancel
          </Button>
          <Button isDisabled={!isValid()} style="highlight" key={1} onClick={onSave}>
            Save
          </Button>
        </GridCell>
      </Grid>
    </GridContainer>
  );
};
