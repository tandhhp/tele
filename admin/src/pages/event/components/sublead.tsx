import { apiAddSubLead, apiDeleteSubLead, apiListSubLead, apiUpdateSubLead } from "@/services/contact";
import { DeleteOutlined, EditOutlined, ManOutlined, UserAddOutlined, WomanOutlined } from "@ant-design/icons";
import { ActionType, DrawerForm, DrawerFormProps, ModalForm, ProFormInstance, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components";
import { useAccess } from "@umijs/max";
import { Button, Col, message, Popconfirm, Row } from "antd";
import { useEffect, useRef, useState } from "react";

type Props = DrawerFormProps & {
    lead: any;
    reload: any;
}

const SubLead: React.FC<Props> = (props) => {

    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const access = useAccess();
    const [subLead, setSubLead] = useState<any>();
    const formRef = useRef<ProFormInstance>();

    useEffect(() => {
        if (subLead) {
            formRef.current?.setFields([
                {
                    name: 'id',
                    value: subLead.id
                },
                {
                    name: 'name',
                    value: subLead.name
                },
                {
                    name: 'phoneNumber',
                    value: subLead.phoneNumber
                },
                {
                    name: 'identityNumber',
                    value: subLead.identityNumber
                },
                {
                    name: 'gender',
                    value: subLead.gender
                },
                {
                    name: 'address',
                    value: subLead.address
                }
            ])
        }
    }, [subLead]);

    useEffect(() => {
        if (props.lead) {
            actionRef.current?.reload();
        }
    }, [props.lead]);

    return (
        <DrawerForm {...props} title="Người đi cùng" submitter={false}>
            <ProTable
                headerTitle={(
                    <Button type="primary" icon={<UserAddOutlined />} onClick={() => setOpen(true)} hidden={!access.event && !access.sales}>Thêm mới</Button>
                )}
                ghost
                actionRef={actionRef}
                search={false}
                request={() => apiListSubLead(props.lead?.id)}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'name',
                        render: (dom, entity) => (
                            <div>{entity.gender === true && (<ManOutlined className='text-blue-500' />)}{entity.gender === false && (<WomanOutlined className='text-red-500' />)} {dom}</div>
                        )
                    },
                    {
                        title: 'Thông tin liên hệ',
                        render: (_, entity) => (
                            <>
                                <div>SDT: {entity.phoneNumber}</div>
                                <div>CCCD: {entity.identityNumber}</div>
                                <div>Địa chỉ: {entity.address}</div>
                            </>
                        )
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        render: (_, entity) => [
                            <Button type="primary" icon={<EditOutlined />} key="edit" size="small" onClick={() => {
                                setSubLead(entity);
                                setOpen(true);
                            }} />,
                            <Popconfirm title="Xác nhận xóa?" key="delete" onConfirm={async () => {
                                await apiDeleteSubLead(entity.id);
                                message.success('Thành công!');
                                actionRef.current?.reload();
                                props.reload();
                            }}>
                                <Button type="primary" danger size="small" icon={<DeleteOutlined />} />
                            </Popconfirm>
                        ],
                        width: 60,
                        hideInTable: !access.event && !access.sales
                    }
                ]}
            />
            <ModalForm open={open} onOpenChange={setOpen} title="Người đi kèm" formRef={formRef} onFinish={async (values) => {
                values.leadId = props.lead?.id;
                if (values.id) {
                    await apiUpdateSubLead(values);
                } else {
                    await apiAddSubLead(values);
                }
                message.success('Thành công!');
                actionRef.current?.reload();
                props.reload();
                setOpen(false);
            }}>
                <Row gutter={16}>
                    <ProFormText name="id" hidden />
                    <Col md={12} xs={12}>
                        <ProFormText name="name" label="Họ và tên" />
                    </Col>
                    <Col md={12} xs={12}>
                        <ProFormText name="phoneNumber" label="Số điện thoại" />
                    </Col>
                    <Col md={12} xs={12}>
                        <ProFormText name="identityNumber" label="CCCD" />
                    </Col>
                    <Col md={12} xs={12}>
                        <ProFormSelect label="Giới tính" name="gender" options={[
                            {
                                label: 'Nam',
                                value: true as any
                            },
                            {
                                label: 'Nữ',
                                value: false as any
                            },
                            {
                                label: 'Khác',
                                value: null
                            }
                        ]} />
                    </Col>
                    <Col md={24} xs={24}>
                        <ProFormText name="address" label="Địa chỉ" />
                    </Col>
                </Row>
            </ModalForm>
        </DrawerForm>
    )
}

export default SubLead;